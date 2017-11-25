// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchivedDirectory.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Spritely.Recipes;

    /// <summary>
    /// Model object for a directory that has been converted into an arvhie file.
    /// </summary>
    public class ArchivedDirectory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchivedDirectory"/> class.
        /// </summary>
        /// <param name="directoryArchiveKind">Kind of archive.</param>
        /// <param name="archiveCompressionKind">Kind of archive compression used.</param>
        /// <param name="archiveFilePath">Path to archive file.</param>
        /// <param name="includeBaseDirectory">Value indicating whether or not the base directory was included.</param>
        /// <param name="entryNameEncoding">Encoding used for the entry names.</param>
        /// <param name="archivedDateTimeUtc">Optional date time in UTC that the file was created; default is <see cref="DateTime.UtcNow" />.</param>
        public ArchivedDirectory(DirectoryArchiveKind directoryArchiveKind, ArchiveCompressionKind archiveCompressionKind, string archiveFilePath, bool includeBaseDirectory, Encoding entryNameEncoding, DateTime archivedDateTimeUtc = default(DateTime))
        {
            new { directoryArchiveKind }.Must().NotBeEqualTo(DirectoryArchiveKind.Invalid).OrThrowFirstFailure();
            new { archiveCompressionKind }.Must().NotBeEqualTo(ArchiveCompressionKind.Invalid).OrThrowFirstFailure();
            new { archiveFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { entryNameEncoding }.Must().NotBeNull().OrThrowFirstFailure();

            this.DirectoryArchiveKind = directoryArchiveKind;
            this.ArchiveFilePath = archiveFilePath;
            this.ArchiveCompressionKind = archiveCompressionKind;
            this.IncludeBaseDirectory = includeBaseDirectory;
            this.EntryNameEncoding = entryNameEncoding;
            this.ArchivedDateTimeUtc = archivedDateTimeUtc == default(DateTime) ? DateTime.UtcNow : archivedDateTimeUtc;
        }

        /// <summary>
        /// Gets the kind of archive.
        /// </summary>
        public DirectoryArchiveKind DirectoryArchiveKind { get; private set; }

        /// <summary>
        /// Gets the kind of archive compression used.
        /// </summary>
        public ArchiveCompressionKind ArchiveCompressionKind { get; private set; }

        /// <summary>
        /// Gets the encoding used for the entry names.
        /// </summary>
        public Encoding EntryNameEncoding { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the base directory was included.
        /// </summary>
        public bool IncludeBaseDirectory { get; private set; }

        /// <summary>
        /// Gets the path to archive file.
        /// </summary>
        public string ArchiveFilePath { get; private set; }

        /// <summary>
        /// Gets the date time in UTC that the file was created.
        /// </summary>
        public DateTime ArchivedDateTimeUtc { get; private set; }
    }

    /// <summary>
    /// Extensions on <see cref="ArchivedDirectory" />
    /// </summary>
    public static class ArchivedDirectoryExtensions
    {
        /// <summary>
        /// Extracts the properties into a collection of <see cref="MetadataItem" />'s.
        /// </summary>
        /// <param name="archivedDirectory"><see cref="ArchivedDirectory" /> to get properties from.</param>
        /// <returns>Collection of <see cref="MetadataItem" />'s</returns>
        public static IReadOnlyCollection<MetadataItem> ToMetadataItemCollection(this ArchivedDirectory archivedDirectory)
        {
            new { archivedDirectory }.Must().NotBeNull().OrThrowFirstFailure();

            var ret = new[]
                          {
                              new MetadataItem(nameof(ArchivedDirectory.DirectoryArchiveKind), archivedDirectory.DirectoryArchiveKind.ToString()),
                              new MetadataItem(nameof(ArchivedDirectory.ArchiveCompressionKind), archivedDirectory.ArchiveCompressionKind.ToString()),
                              new MetadataItem(nameof(ArchivedDirectory.IncludeBaseDirectory), archivedDirectory.IncludeBaseDirectory.ToString()),
                              new MetadataItem(nameof(ArchivedDirectory.EntryNameEncoding), archivedDirectory.EntryNameEncoding.ToString()),
                              new MetadataItem(nameof(ArchivedDirectory.ArchivedDateTimeUtc), DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                          };

            return ret;
        }
    }
}