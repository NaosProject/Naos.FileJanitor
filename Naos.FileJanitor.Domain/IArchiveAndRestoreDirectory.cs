// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArchiveAndRestoreDirectory.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to combine <see cref="IArchiveDirectory" /> and <see cref="IRestoreDirectory" />.
    /// </summary>
    public interface IArchiveAndRestoreDirectory : IArchiveDirectory, IRestoreDirectory
    {
    }

    /// <summary>
    /// Interface to expose the <see cref="DirectoryArchiveKind" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Archiver", Justification = "Spelling/name is correct.")]
    public interface IAmKindOfArchiver
    {
        /// <summary>
        /// Gets the kind of archive.
        /// </summary>
        DirectoryArchiveKind DirectoryArchiveKind { get; }
    }

    /// <summary>
    /// Interface to expose logic to archive a directory to a file.
    /// </summary>
    public interface IArchiveDirectory : IAmKindOfArchiver
    {
        /// <summary>
        /// Archive a directory to an archive file.
        /// </summary>
        /// <param name="sourcePath">Directory path to archive.</param>
        /// <param name="targetFilePath">File path to produce archive file at.</param>
        /// <param name="includeBaseDirectory">Optional value indicating whether or not to include the base directory when constructing the archive; DEFAULT is true.</param>
        /// <param name="entryNameEncoding">Optional encoding to use for the entry file names; DEFAULT is <see cref="Encoding.UTF8" />.</param>
        /// <returns><see cref="ArchivedDirectory" /> with details of the archive.</returns>
        Task<ArchivedDirectory> ArchiveDirectoryAsync(string sourcePath, string targetFilePath, bool includeBaseDirectory = true, Encoding entryNameEncoding = null);
    }

    /// <summary>
    /// Interface to expose logic to restore a directory from an archived file.
    /// </summary>
    public interface IRestoreDirectory : IAmKindOfArchiver
    {
        /// <summary>
        /// Restores a directory from an archive file.
        /// </summary>
        /// <param name="archivedDirectory">Description of the archive, including the file path.</param>
        /// <param name="targetPath">Target path to restore directory.</param>
        /// <returns>Task for async.</returns>
        Task RestoreDirectoryAsync(ArchivedDirectory archivedDirectory, string targetPath);
    }
}
