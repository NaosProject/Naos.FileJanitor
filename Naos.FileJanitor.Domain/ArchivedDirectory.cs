// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchivedDirectory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Equality.Recipes;

    /// <summary>
    /// Model object for a directory that has been converted into an archive file.
    /// </summary>
    public class ArchivedDirectory : IEquatable<ArchivedDirectory>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchivedDirectory"/> class.
        /// </summary>
        /// <param name="directoryArchiveKind">Kind of archive.</param>
        /// <param name="archiveCompressionKind">Kind of archive compression used.</param>
        /// <param name="archiveFilePath">Path to archive file.</param>
        /// <param name="includeBaseDirectory">Value indicating whether or not the base directory was included.</param>
        /// <param name="entryNameEncodingWebName">Encoding used for the entry names.</param>
        /// <param name="archivedDateTimeUtc">Optional date time in UTC that the file was created; default is <see cref="DateTime.UtcNow" />.</param>
        public ArchivedDirectory(DirectoryArchiveKind directoryArchiveKind, ArchiveCompressionKind archiveCompressionKind, string archiveFilePath, bool includeBaseDirectory, string entryNameEncodingWebName, DateTime archivedDateTimeUtc = default(DateTime))
        {
            new { directoryArchiveKind }.AsArg().Must().NotBeEqualTo(DirectoryArchiveKind.Invalid);
            new { archiveCompressionKind }.AsArg().Must().NotBeEqualTo(ArchiveCompressionKind.Invalid);
            new { archiveFilePath }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { entryNameEncoding = entryNameEncodingWebName }.AsArg().Must().NotBeNull();

            this.DirectoryArchiveKind = directoryArchiveKind;
            this.ArchiveFilePath = archiveFilePath;
            this.ArchiveCompressionKind = archiveCompressionKind;
            this.IncludeBaseDirectory = includeBaseDirectory;
            this.EntryNameEncodingWebName = entryNameEncodingWebName;
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
        public string EntryNameEncodingWebName { get; private set; }

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

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(ArchivedDirectory first, ArchivedDirectory second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.DirectoryArchiveKind == second.DirectoryArchiveKind &&
                   first.ArchiveCompressionKind == second.ArchiveCompressionKind &&
                   string.Equals(first.EntryNameEncodingWebName, second.EntryNameEncodingWebName, StringComparison.OrdinalIgnoreCase) &&
                   first.IncludeBaseDirectory == second.IncludeBaseDirectory &&
                   string.Equals(first.ArchiveFilePath, second.ArchiveFilePath, StringComparison.OrdinalIgnoreCase) &&
                   first.ArchivedDateTimeUtc == second.ArchivedDateTimeUtc;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(ArchivedDirectory first, ArchivedDirectory second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(ArchivedDirectory other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ArchivedDirectory);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.DirectoryArchiveKind)
            .Hash(this.ArchiveCompressionKind)
            .Hash(this.EntryNameEncodingWebName)
            .Hash(this.IncludeBaseDirectory)
            .Hash(this.ArchiveFilePath)
            .Hash(this.ArchivedDateTimeUtc)
            .Value;
    }

    /// <summary>
    /// Extensions on <see cref="ArchivedDirectory" />.
    /// </summary>
    public static class ArchivedDirectoryExtensions
    {
        /// <summary>
        /// Extracts the properties into a collection of <see cref="MetadataItem" />'s.
        /// </summary>
        /// <param name="archivedDirectory"><see cref="ArchivedDirectory" /> to get properties from.</param>
        /// <returns>Collection of <see cref="MetadataItem" />'s.</returns>
        public static IReadOnlyCollection<MetadataItem> ToMetadataItemCollection(this ArchivedDirectory archivedDirectory)
        {
            new { archivedDirectory }.AsArg().Must().NotBeNull();

            var ret = new[]
                          {
                              new MetadataItem(nameof(ArchivedDirectory.DirectoryArchiveKind), archivedDirectory.DirectoryArchiveKind.ToString()),
                              new MetadataItem(nameof(ArchivedDirectory.ArchiveCompressionKind), archivedDirectory.ArchiveCompressionKind.ToString()),
                              new MetadataItem(nameof(ArchivedDirectory.IncludeBaseDirectory), archivedDirectory.IncludeBaseDirectory.ToString()),
                              new MetadataItem(nameof(ArchivedDirectory.EntryNameEncodingWebName), archivedDirectory.EntryNameEncodingWebName),
                              new MetadataItem(nameof(ArchivedDirectory.ArchivedDateTimeUtc), DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                          };

            return ret;
        }
    }
}
