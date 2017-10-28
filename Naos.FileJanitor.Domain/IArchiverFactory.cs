// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArchiverFactory.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    /// <summary>
    /// Model object for a directory that has been converted into an arvhie file.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Archiver", Justification = "Spelling/name is correct.")]
    public interface IArchiverFactory
    {
        /// <summary>
        /// Builds an implementation of <see cref="IArchiveAndRestoreDirectory" /> from provided <see cref="ArchivedDirectory" />.
        /// </summary>
        /// <param name="archivedDirectory">Kind to use for determination.</param>
        /// <returns>New determined impelemenation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Archiver", Justification = "Spelling/name is correct.")]
        IArchiveAndRestoreDirectory BuildArchiver(ArchivedDirectory archivedDirectory);

        /// <summary>
        /// Builds an implementation of <see cref="IArchiveAndRestoreDirectory" /> from provided <see cref="DirectoryArchiveKind" /> and <see cref="ArchiveCompressionKind" />.
        /// </summary>
        /// <param name="directoryArchiveKind">Kind to use for determination.</param>
        /// <param name="archiveCompressionKind">Kind of compression to use.</param>
        /// <returns>New determined impelemenation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Archiver", Justification = "Spelling/name is correct.")]
        IArchiveAndRestoreDirectory BuildArchiver(DirectoryArchiveKind directoryArchiveKind, ArchiveCompressionKind archiveCompressionKind);
    }
}