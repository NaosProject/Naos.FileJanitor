// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchiveDirectoryMessageHandler.cs" company="Naos">
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
    /// Message handler for <see cref="ArchiveDirectoryMessage" />.
    /// </summary>
    public class ArchiveDirectoryMessageHandler : MessageHandlerBase<ArchiveDirectoryMessage>, IShareFilePath, IShareUserDefinedMetadata
    {
        /// <inheritdoc cref="MessageHandlerBase{T}" />
        public override async Task HandleAsync(ArchiveDirectoryMessage message)
        {
            using (var log = Log.Enter(() => new { Message = message, message.FilePath }))
            {
                new { message.FilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
                new { message.TargetFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

                Directory.Exists(message.FilePath).Named(Invariant($"SourceDirectory-MustExist-{message.FilePath ?? "[NULL]"}")).Must().BeTrue().OrThrowFirstFailure();
                File.Exists(message.TargetFilePath).Named(Invariant($"TargetFile-MustNotExist-{message.TargetFilePath ?? "[NULL]"}")).Must().BeFalse().OrThrowFirstFailure();

                log.Trace(() => Invariant($"Start archiving directory using; {nameof(DirectoryArchiveKind)}: {message.DirectoryArchiveKind}, {nameof(ArchiveCompressionKind)}: {message.ArchiveCompressionKind}"));

                var archiver = ArchiverFactory.Instance.BuildArchiver(message.DirectoryArchiveKind, message.ArchiveCompressionKind);
                new { archiver }.Must().NotBeNull().OrThrowFirstFailure();

                var archivedDirectory = await archiver.ArchiveDirectory(message.FilePath, message.TargetFilePath);

                this.FilePath = await Task.FromResult(message.TargetFilePath); // share compressed file

                this.UserDefinedMetadata = message.UserDefinedMetadata.Concat(
                    new[]
                        {
                            new MetadataItem(nameof(ArchivedDirectory.DirectoryArchiveKind), archivedDirectory.DirectoryArchiveKind.ToString()),
                            new MetadataItem(nameof(ArchivedDirectory.ArchiveCompressionKind), archivedDirectory.ArchiveCompressionKind.ToString()),
                            new MetadataItem(nameof(ArchivedDirectory.IncludeBaseDirectory), archivedDirectory.IncludeBaseDirectory.ToString()),
                            new MetadataItem(nameof(ArchivedDirectory.EntryNameEncoding), archivedDirectory.EntryNameEncoding.ToString()),
                            new MetadataItem(nameof(ArchivedDirectory.ArchivedDateTimeUtc), DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                        }).ToArray();

                log.Trace(() => Invariant($"Finished archiving directory to {message.TargetFilePath}."));
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }

        /// <inheritdoc />
        public MetadataItem[] UserDefinedMetadata { get; set; }
    }
}
