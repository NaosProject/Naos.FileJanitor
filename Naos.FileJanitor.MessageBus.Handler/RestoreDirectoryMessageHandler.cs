// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestoreDirectoryMessageHandler.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.Core;
    using Naos.FileJanitor.Domain;
    using Naos.FileJanitor.MessageBus.Scheduler;
    using Naos.MessageBus.Domain;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Message handler for <see cref="RestoreDirectoryMessage" />.
    /// </summary>
    public class RestoreDirectoryMessageHandler : MessageHandlerBase<RestoreDirectoryMessage>, IShareFilePath
    {
        /// <inheritdoc cref="MessageHandlerBase{T}" />
        public override async Task HandleAsync(RestoreDirectoryMessage message)
        {
            using (var log = Log.Enter(() => new { Message = message, message.FilePath }))
            {
                new { message.FilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
                new { message.TargetFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

                File.Exists(message.FilePath).Named(Invariant($"SourceFile-MustExist-{message.FilePath ?? "[NULL]"}")).Must().BeTrue().OrThrowFirstFailure();
                Directory.Exists(message.TargetFilePath).Named(Invariant($"TargetDirectory-MustNotExist-{message.TargetFilePath ?? "[NULL]"}")).Must().BeFalse().OrThrowFirstFailure();

                var directoryArchiveKindRaw = message.UserDefinedMetadata.SingleOrDefault(_ => _.Key == nameof(ArchivedDirectory.DirectoryArchiveKind))?.Value
                                    ?? throw new ArgumentException(Invariant($"{nameof(message)}.{nameof(message.UserDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.DirectoryArchiveKind)}"));
                var directoryArchiveKind = (DirectoryArchiveKind)Enum.Parse(typeof(DirectoryArchiveKind), directoryArchiveKindRaw, true);

                var archiveCompressionKindRaw = message.UserDefinedMetadata.SingleOrDefault(_ => _.Key == nameof(ArchivedDirectory.ArchiveCompressionKind))?.Value
                                    ?? throw new ArgumentException(Invariant($"{nameof(message)}.{nameof(message.UserDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.ArchiveCompressionKind)}"));
                var archiveCompressionKind = (ArchiveCompressionKind)Enum.Parse(typeof(ArchiveCompressionKind), archiveCompressionKindRaw, true);

                var includeBaseDirectoryRaw = message.UserDefinedMetadata.SingleOrDefault(_ => _.Key == nameof(ArchivedDirectory.IncludeBaseDirectory))?.Value
                                    ?? throw new ArgumentException(Invariant($"{nameof(message)}.{nameof(message.UserDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.IncludeBaseDirectory)}"));
                var includeBaseDirectory = bool.Parse(includeBaseDirectoryRaw);

                var entryNameEncodingRaw = message.UserDefinedMetadata.SingleOrDefault(_ => _.Key == nameof(ArchivedDirectory.EntryNameEncoding))?.Value
                                    ?? throw new ArgumentException(Invariant($"{nameof(message)}.{nameof(message.UserDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.EntryNameEncoding)}"));
                var entryNameEncoding = Encoding.GetEncoding(entryNameEncodingRaw);
                entryNameEncoding.Named(Invariant($"EntryNameEncoding-ParsedFrom-{nameof(message)}.{nameof(message.UserDefinedMetadata)}-key-{nameof(ArchivedDirectory.EntryNameEncoding)}")).Must().NotBeNull().OrThrowFirstFailure();

                var archivedDateTimeUtcRaw = message.UserDefinedMetadata.SingleOrDefault(_ => _.Key == nameof(ArchivedDirectory.ArchivedDateTimeUtc))?.Value
                                    ?? throw new ArgumentException(Invariant($"{nameof(message)}.{nameof(message.UserDefinedMetadata)} is missing value for {nameof(ArchivedDirectory.EntryNameEncoding)}"));
                var archivedDateTimeUtc = DateTime.Parse(archivedDateTimeUtcRaw);
                archivedDateTimeUtc.Named(Invariant($"ArchivedDateTimeUtc-ParsedFrom-{nameof(message)}.{nameof(message.UserDefinedMetadata)}-key-{nameof(ArchivedDirectory.ArchivedDateTimeUtc)}")).Must().NotBeEqualTo(default(DateTime)).OrThrowFirstFailure();

                log.Trace(() => Invariant($"Start restoring directory using; {nameof(DirectoryArchiveKind)}: {directoryArchiveKind}, {nameof(ArchiveCompressionKind)}: {archiveCompressionKind}"));

                var archiver = ArchiverFactory.Instance.BuildArchiver(directoryArchiveKind, archiveCompressionKind);
                new { archiver }.Must().NotBeNull().OrThrowFirstFailure();

                var archivedDirectory = new ArchivedDirectory(directoryArchiveKind, archiveCompressionKind, message.FilePath, includeBaseDirectory, entryNameEncoding, archivedDateTimeUtc);
                await archiver.RestoreDirectory(archivedDirectory, message.TargetFilePath);

                this.FilePath = await Task.FromResult(message.TargetFilePath); // share compressed file

                log.Trace(() => Invariant($"Finished restoring directory to {message.TargetFilePath}."));
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
