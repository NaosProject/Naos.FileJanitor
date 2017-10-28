// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZipFileArchiver.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Core
{
    using System;
    using System.IO.Compression;
    using System.Text;
    using System.Threading.Tasks;

    using Naos.FileJanitor.Domain;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Implementation of <see cref="IArchiveAndRestoreDirectory" /> using <see cref="ZipFile" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Archiver", Justification = "Spelling/name is correct.")]
    public class ZipFileArchiver : IArchiveAndRestoreDirectory
    {
        /// <summary>
        /// Default encoding used for the names of the entries.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Want this to be a read only field.")]
        public static readonly Encoding DefaultEntryNameEncoding = Encoding.UTF8;

        private readonly ArchiveCompressionKind archiveCompressionKind;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipFileArchiver"/> class.
        /// </summary>
        /// <param name="archiveCompressionKind">Compression kind to use.</param>
        public ZipFileArchiver(ArchiveCompressionKind archiveCompressionKind)
        {
            new { archiveCompressionKind }.Must().NotBeEqualTo(ArchiveCompressionKind.Invalid).OrThrowFirstFailure();

            this.archiveCompressionKind = archiveCompressionKind;
        }

        /// <inheritdoc cref="IAmKindOfArchiver" />
        public DirectoryArchiveKind DirectoryArchiveKind => DirectoryArchiveKind.DotNetZipFile;

        /// <inheritdoc cref="IArchiveDirectory" />
        public async Task<ArchivedDirectory> ArchiveDirectory(string sourcePath, string targetFilePath, bool includeBaseDirectory = true, Encoding entryNameEncoding = null)
        {
            new { sourcePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { targetFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            var localEntryNameEncoding = entryNameEncoding ?? DefaultEntryNameEncoding;

            var compressionLevel = GetCompressionLevel(this.archiveCompressionKind);

            ZipFile.CreateFromDirectory(sourcePath, targetFilePath, compressionLevel, includeBaseDirectory, localEntryNameEncoding);

            var ret = new ArchivedDirectory(this.DirectoryArchiveKind, this.archiveCompressionKind, targetFilePath, includeBaseDirectory, localEntryNameEncoding);
            return await Task.FromResult(ret);
        }

        /// <inheritdoc cref="IRestoreDirectory" />
        public async Task RestoreDirectory(ArchivedDirectory archivedDirectory, string targetPath)
        {
            new { archivedDirectory }.Must().NotBeNull().OrThrowFirstFailure();
            new { targetPath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            ZipFile.ExtractToDirectory(archivedDirectory.ArchiveFilePath, targetPath, archivedDirectory.EntryNameEncoding);
            await Task.Run(() => { /* no-op */ });
        }

        private static CompressionLevel GetCompressionLevel(ArchiveCompressionKind archiveCompressionKind)
        {
            switch (archiveCompressionKind)
            {
                case ArchiveCompressionKind.None: return CompressionLevel.NoCompression;
                case ArchiveCompressionKind.Smallest: return CompressionLevel.Optimal;
                case ArchiveCompressionKind.Fastest: return CompressionLevel.Fastest;
                default: throw new NotSupportedException(Invariant($"Provided {nameof(archiveCompressionKind)} - {archiveCompressionKind} - is not supported to convert into - {nameof(ArchiveCompressionKind)}."));
            }
        }
    }
}