// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilePathJanitor.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using static System.FormattableString;

    /// <summary>
    /// Tools for helping with cleaning up paths.
    /// </summary>
    public static class FilePathJanitor
    {
        /// <summary>
        /// Removes old files.
        /// </summary>
        /// <param name="rootPath">The root path to evaluate (must be a directory).</param>
        /// <param name="retentionWindow">The time to retain files (in format dd:hh:mm).</param>
        /// <param name="recursive">Whether or not to evaluate files recursively on the path.</param>
        /// <param name="deleteEmptyDirectories">Whether or not to delete directories that are or become empty during cleanup.</param>
        /// <param name="dateRetrievalStrategy">The date retrieval strategy to use on files.</param>
        /// <param name="announcer">Optional announcer to log messages during run.</param>
        public static void Cleanup(
            string rootPath,
            TimeSpan retentionWindow,
            bool recursive,
            bool deleteEmptyDirectories,
            DateRetrievalStrategy dateRetrievalStrategy,
            Action<Func<object>> announcer = null)
        {
            void NullAnnounce(Func<object> announcement)
            {
                /* no-op */
            }

            var localAnnouncer = announcer ?? NullAnnounce;
            var recursiveString = (recursive ? string.Empty : "not ") + nameof(recursive);
            var deleteEmptyString = (deleteEmptyDirectories ? string.Empty : "don't ") + nameof(deleteEmptyDirectories);
            localAnnouncer(() => Invariant($"Started cleaning-up the directory {rootPath}, {retentionWindow}, {dateRetrievalStrategy}, {recursiveString}, {deleteEmptyString}."));

            if (!File.GetAttributes(rootPath).HasFlag(FileAttributes.Directory))
            {
                throw new ArgumentException("Root path must be a directory.");
            }

            if (!Directory.Exists(rootPath))
            {
                throw new ArgumentException("Root path: " + rootPath + " does not exist.");
            }

            var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            localAnnouncer(() => "Identifying all files that should be considered for cleanup.");
            var files = Directory.GetFiles(rootPath, "*", searchOptions);

            localAnnouncer(() => "Filtering to files that are outside the retention window.");
            var cutoff = DateTime.UtcNow.Subtract(retentionWindow);
            var filesToDelete = FilterFilesToBeforeCutOff(files, cutoff, dateRetrievalStrategy);

            foreach (var fileToDelete in filesToDelete)
            {
                var localFileToDelete = fileToDelete;
                localAnnouncer(
                    () =>
                    "File: " + localFileToDelete + " is being removed because it's outside of the retention window.");
                File.Delete(fileToDelete);
            }

            if (deleteEmptyDirectories)
            {
                localAnnouncer(() => "Removing any empty directories.");
                foreach (
                    var directoryPath in Directory.GetDirectories(rootPath, "*", searchOptions))
                {
                    var directory = new DirectoryInfo(directoryPath);
                    if (!directory.GetFiles().Any())
                    {
                        var localDirectoryPath = directoryPath;
                        localAnnouncer(
                            () => "Directory: " + localDirectoryPath + " is being removed because it's empty.");
                        directory.Delete(recursive);
                    }
                }
            }

            localAnnouncer(() => "Completed cleaning-up the directory.");
        }

        private static string[] FilterFilesToBeforeCutOff(string[] filePaths, DateTime cutoffInUtc, DateRetrievalStrategy dateRetrievalStrategy)
        {
            var ret = new List<string>();
            foreach (var filePath in filePaths)
            {
                var file = new FileInfo(filePath);
                DateTime compareDateUtc;
                switch (dateRetrievalStrategy)
                {
                    case DateRetrievalStrategy.LastUpdateDate:
                        compareDateUtc = file.LastWriteTimeUtc;
                        break;
                    case DateRetrievalStrategy.LastAccessDate:
                        compareDateUtc = file.LastAccessTimeUtc;
                        break;
                    case DateRetrievalStrategy.CreateDate:
                        compareDateUtc = file.CreationTimeUtc;
                        break;
                    default:
                        throw new ArgumentException("Unsupported DateRetrievalStrategy: " + dateRetrievalStrategy);
                }

                if (compareDateUtc < cutoffInUtc)
                {
                    ret.Add(filePath);
                }
            }

            return ret.ToArray();
        }
    }
}
