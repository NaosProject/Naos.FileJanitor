// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileExchanger.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.S3
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using ByteSizeLib;
    using Naos.AWS.Domain;
    using Naos.FileJanitor.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using Spritely.Redo;
    using static System.FormattableString;

    /// <summary>
    /// Tools for helping with storing and retrieving files with a <see cref="IManageFiles"/>.
    /// </summary>
    public static class FileExchanger
    {
        /// <summary>
        /// Store a file using the provided <see cref="IManageFiles" />.
        /// </summary>
        /// <param name="fileManager"><see cref="IManageFiles" /> implementation to use.</param>
        /// <param name="filePath">File to store.</param>
        /// <param name="containerLocation">Location of container to use.</param>
        /// <param name="container">Container to use.</param>
        /// <param name="key">Optional key to store as; DEFAULT is file name.</param>
        /// <param name="userDefinedMetadata">Optional user metadata.</param>
        /// <param name="hashingAlgorithmNames">Optional <see cref="HashAlgorithmName" />'s.</param>
        /// <returns>Task for async.</returns>
        public static async Task StoreFile(IManageFiles fileManager, string filePath, string containerLocation, string container, string key = null, IReadOnlyCollection<MetadataItem> userDefinedMetadata = null, IReadOnlyCollection<string> hashingAlgorithmNames = null)
        {
            var hashAlgorithmNames = (hashingAlgorithmNames ?? new string[0])
                .Select(_ => string.IsNullOrWhiteSpace(_) ? default(HashAlgorithmName) : new HashAlgorithmName(_)).Distinct()
                .Where(_ => _ != default(HashAlgorithmName)).ToList();

            var localUserDefinedMetadata = (userDefinedMetadata ?? new MetadataItem[0]).ToReadOnlyDictionary();

            var fileSize = ByteSize.FromBytes(new FileInfo(filePath).Length);
            var attemptWaitTimeMultiplier = TimeSpan.FromSeconds(fileSize.MegaBytes * 0.001);
            var minimumAttemptWaitTimeMultiplier = TimeSpan.FromSeconds(5);
            if (attemptWaitTimeMultiplier < minimumAttemptWaitTimeMultiplier)
            {
                attemptWaitTimeMultiplier = minimumAttemptWaitTimeMultiplier;
            }

            await Using.LinearBackOff(attemptWaitTimeMultiplier).WithMaxRetries(3).RunAsync(
                () => fileManager.UploadFileAsync(
                    containerLocation,
                    container,
                    key ?? Path.GetFileName(filePath),
                    filePath,
                    hashAlgorithmNames,
                    localUserDefinedMetadata)).Now();
        }

        /// <summary>
        /// Store a file using the provided <see cref="IManageFiles" />.
        /// </summary>
        /// <param name="fileManager"><see cref="IManageFiles" /> implementation to use.</param>
        /// <param name="directoryPath">File to store.</param>
        /// <param name="directoryArchiveKind"><see cref="DirectoryArchiveKind" /> to use.</param>
        /// <param name="archiveCompressionKind"><see cref="ArchiveCompressionKind" /> to use.</param>
        /// <param name="includeBaseDirectory">Include base directory in archive.</param>
        /// <param name="entryNameEncoding"><see cref="Encoding" /> to use for entry names.</param>
        /// <param name="containerLocation">Location of container to use.</param>
        /// <param name="container">Container to use.</param>
        /// <param name="key">Optional key to store as; DEFAULT is directory name.</param>
        /// <param name="userDefinedMetadata">Optional user metadata.</param>
        /// <param name="hashingAlgorithmNames">Optional <see cref="HashAlgorithmName" />'s.</param>
        /// <returns>Task for async.</returns>
        public static async Task StoreDirectory(IManageFiles fileManager, string directoryPath, DirectoryArchiveKind directoryArchiveKind, ArchiveCompressionKind archiveCompressionKind, bool includeBaseDirectory, Encoding entryNameEncoding, string containerLocation, string container, string key = null, IReadOnlyCollection<MetadataItem> userDefinedMetadata = null, string[] hashingAlgorithmNames = null)
        {
            var archiver = ArchiverFactory.Instance.BuildArchiver(directoryArchiveKind, archiveCompressionKind);
            var tempFile = Path.Combine(Path.GetDirectoryName(directoryPath) ?? Path.GetTempPath(), "FileJanitor-StoreDirectory-" + Guid.NewGuid() + ".tmp");
            try
            {
                var archivedDirectory = await archiver.ArchiveDirectoryAsync(directoryPath, tempFile, includeBaseDirectory, entryNameEncoding);

                var localUserDefinedMetadata = (userDefinedMetadata ?? new MetadataItem[0]).Concat(archivedDirectory.ToMetadataItemCollection()).ToArray();

                await StoreFile(fileManager, tempFile, containerLocation, container, key ?? Path.GetFileName(directoryPath), localUserDefinedMetadata, hashingAlgorithmNames);
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(tempFile) && File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        /// <summary>
        /// Store a file using the provided <see cref="IManageFiles" />.
        /// </summary>
        /// <param name="fileManager"><see cref="IManageFiles" /> implementation to use.</param>
        /// <param name="containerLocation">Location of container to use.</param>
        /// <param name="container">Container to use.</param>
        /// <param name="keyPrefixSearchPattern">Key prefix to use for searching.</param>
        /// <param name="multipleKeysFoundStrategy"><see cref="MultipleKeysFoundStrategy" /> to use.</param>
        /// <returns>Task for async.</returns>
        public static async Task<FileLocation> FindFile(IManageFiles fileManager, string containerLocation, string container, string keyPrefixSearchPattern, MultipleKeysFoundStrategy multipleKeysFoundStrategy)
        {
            var files =
                await
                    Using.LinearBackOff(TimeSpan.FromSeconds(5))
                        .WithMaxRetries(3)
                        .RunAsync(() => fileManager.ListFilesAsync(containerLocation, container, keyPrefixSearchPattern))
                        .Now();

            if (multipleKeysFoundStrategy == MultipleKeysFoundStrategy.SingleMatchExpectedThrow && files.Count > 1)
            {
                throw new InvalidDataException("Expected a single S3Object => Prefix Search: " + (keyPrefixSearchPattern ?? "[NULL]") + ", Count: " + files.Count);
            }

            var keys = files.Select(_ => _.KeyName).ToList();
            switch (multipleKeysFoundStrategy)
            {
                case MultipleKeysFoundStrategy.FirstSortedAscending:
                    keys = keys.OrderBy(_ => _).ToList();
                    break;
                case MultipleKeysFoundStrategy.FirstSortedDescending:
                    keys = keys.OrderByDescending(_ => _).ToList();
                    break;
                default:
                    throw new NotSupportedException("Unsupported multiple found strategy => " + multipleKeysFoundStrategy);
            }

            var key = keys.FirstOrDefault();
            if (key == null)
            {
                throw new FileNotFoundException(
                    $"Could not find an S3 Object => region: {containerLocation}, bucket: {container}, KeyPrefixSearchPattern: {keyPrefixSearchPattern}");
            }

            return new FileLocation { ContainerLocation = containerLocation, Container = container, Key = key };
        }

        /// <summary>
        /// Get metadata of a file using the provided <see cref="IManageFiles" />.
        /// </summary>
        /// <param name="fileManager"><see cref="IManageFiles" /> implementation to use.</param>
        /// <param name="containerLocation">Location of container to use.</param>
        /// <param name="container">Container to use.</param>
        /// <param name="key">Key to use.</param>
        /// <returns>Task for async.</returns>
        public static async Task<IReadOnlyCollection<MetadataItem>> FetchMetadata(IManageFiles fileManager, string containerLocation, string container, string key)
        {
            var metadata = await Using.LinearBackOff(TimeSpan.FromSeconds(5)).WithMaxRetries(3)
                               .RunAsync(
                                   () => fileManager.GetFileMetadataAsync(
                                       containerLocation,
                                       container,
                                       key)).Now();

            return metadata.ToReadOnlyCollection();
        }

        /// <summary>
        /// Get a file using the provided <see cref="IManageFiles" />.
        /// </summary>
        /// <param name="fileManager"><see cref="IManageFiles" /> implementation to use.</param>
        /// <param name="containerLocation">Location of container to use.</param>
        /// <param name="container">Container to use.</param>
        /// <param name="key">Key to use.</param>
        /// <param name="filePath">Target file path to use.</param>
        /// <returns>Task for async.</returns>
        public static async Task FetchFile(IManageFiles fileManager, string containerLocation, string container, string key, string filePath)
        {
            await Using.LinearBackOff(TimeSpan.FromSeconds(5)).WithMaxRetries(3)
                .RunAsync(() => fileManager.DownloadFileAsync(containerLocation, container, key, filePath)).Now();
        }

        /// <summary>
        /// Restore a download archive to a directory.
        /// </summary>
        /// <param name="filePath">File path of archive file.</param>
        /// <param name="targetFilePath">File path to restore to.</param>
        /// <param name="userDefinedMetadata">Metadata of the file to use for extracting archive details.</param>
        /// <returns>Task for async.</returns>
        public static async Task RestoreDownload(string filePath, string targetFilePath, IReadOnlyCollection<MetadataItem> userDefinedMetadata)
        {
            new { filePath }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { targetFilePath }.AsArg().Must().NotBeNullNorWhiteSpace();

            File.Exists(filePath).AsOp(Invariant($"SourceFile-MustExist-{filePath ?? "[NULL]"}")).Must().BeTrue();
            Directory.Exists(targetFilePath).AsOp(Invariant($"TargetDirectory-MustNotExist-{targetFilePath ?? "[NULL]"}")).Must().BeFalse();

            var directoryArchiveKindRaw = userDefinedMetadata.SingleOrDefault(_ => _.Key.ToLower() == nameof(ArchivedDirectory.DirectoryArchiveKind).ToLower())?.Value
                                ?? throw new ArgumentException(Invariant($"{nameof(userDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.DirectoryArchiveKind)}"));
            var directoryArchiveKind = (DirectoryArchiveKind)Enum.Parse(typeof(DirectoryArchiveKind), directoryArchiveKindRaw, true);

            var archiveCompressionKindRaw = userDefinedMetadata.SingleOrDefault(_ => _.Key.ToLower() == nameof(ArchivedDirectory.ArchiveCompressionKind).ToLower())?.Value
                                ?? throw new ArgumentException(Invariant($"{nameof(userDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.ArchiveCompressionKind)}"));
            var archiveCompressionKind = (ArchiveCompressionKind)Enum.Parse(typeof(ArchiveCompressionKind), archiveCompressionKindRaw, true);

            var includeBaseDirectoryRaw = userDefinedMetadata.SingleOrDefault(_ => _.Key.ToLower() == nameof(ArchivedDirectory.IncludeBaseDirectory).ToLower())?.Value
                                ?? throw new ArgumentException(Invariant($"{nameof(userDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.IncludeBaseDirectory)}"));
            var includeBaseDirectory = bool.Parse(includeBaseDirectoryRaw);

            var entryNameEncodingRaw = userDefinedMetadata.SingleOrDefault(_ => _.Key.ToLower() == nameof(ArchivedDirectory.EntryNameEncodingWebName).ToLower())?.Value
                                ?? throw new ArgumentException(Invariant($"{nameof(userDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.EntryNameEncodingWebName)}"));
            var entryNameEncoding = Encoding.GetEncoding(entryNameEncodingRaw);
            entryNameEncoding.AsOp(Invariant($"EntryNameEncoding-ParsedFrom-{nameof(userDefinedMetadata)}-key-{nameof(ArchivedDirectory.EntryNameEncodingWebName)}"))
                .Must().NotBeNull();

            var archivedDateTimeUtcRaw = userDefinedMetadata.SingleOrDefault(_ => _.Key.ToLower() == nameof(ArchivedDirectory.ArchivedDateTimeUtc).ToLower())?.Value
                                ?? throw new ArgumentException(Invariant($"{nameof(userDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.EntryNameEncodingWebName)}"));
            var archivedDateTimeUtc = DateTime.Parse(archivedDateTimeUtcRaw);
            archivedDateTimeUtc.AsArg(Invariant($"ArchivedDateTimeUtc-ParsedFrom-{nameof(userDefinedMetadata)}-key-{nameof(ArchivedDirectory.ArchivedDateTimeUtc)}")).Must().NotBeEqualTo(default(DateTime));

            var archiver = ArchiverFactory.Instance.BuildArchiver(directoryArchiveKind, archiveCompressionKind);
            new { archiver }.AsOp().Must().NotBeNull();

            var archivedDirectory = new ArchivedDirectory(directoryArchiveKind, archiveCompressionKind, filePath, includeBaseDirectory, entryNameEncoding.WebName, archivedDateTimeUtc);
            await archiver.RestoreDirectoryAsync(archivedDirectory, targetFilePath);
        }
    }
}
