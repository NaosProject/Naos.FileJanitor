// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchiverFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    using System;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Default implementation of <see cref="IArchiverFactory" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Archiver", Justification = "Spelling/name is correct.")]
    public class ArchiverFactory : IArchiverFactory
    {
        private static readonly ArchiverFactory InternalInstance = new ArchiverFactory();

        /// <summary>
        /// Gets the singleton entry point to the code.
        /// </summary>
        public static IArchiverFactory Instance => InternalInstance;

        private readonly object sync = new object();

        private ArchiverFactory()
        {
            /* no-op to make sure this can only be accessed via instance property */
        }

        /// <inheritdoc cref="IArchiverFactory" />
        public IArchiveAndRestoreDirectory BuildArchiver(ArchivedDirectory archivedDirectory)
        {
            new { archivedDirectory }.Must().NotBeNull();

            return this.BuildArchiver(archivedDirectory.DirectoryArchiveKind, archivedDirectory.ArchiveCompressionKind);
        }

        /// <inheritdoc cref="IArchiverFactory" />
        public IArchiveAndRestoreDirectory BuildArchiver(DirectoryArchiveKind directoryArchiveKind, ArchiveCompressionKind archiveCompressionKind)
        {
            new { directoryArchiveKind }.Must().NotBeEqualTo(DirectoryArchiveKind.Invalid);
            new { archiveCompressionKind }.Must().NotBeEqualTo(ArchiveCompressionKind.Invalid);

            lock (this.sync)
            {
                switch (directoryArchiveKind)
                {
                    case DirectoryArchiveKind.DotNetZipFile: return new ZipFileArchiver(archiveCompressionKind);
                    default: throw new NotSupportedException(Invariant($"Provided {nameof(directoryArchiveKind)} - {directoryArchiveKind} - is not supported."));
                }
            }
        }
    }
}
