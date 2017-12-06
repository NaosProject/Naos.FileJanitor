// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilePathJanitor.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.Domain;

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
        public static void Cleanup(
            string rootPath,
            TimeSpan retentionWindow,
            bool recursive,
            bool deleteEmptyDirectories,
            DateRetrievalStrategy dateRetrievalStrategy)
        {
            using (var log = Log.Enter(() => new { rootPath }))
            {
                var recursiveString = (recursive ? string.Empty : "not ") + nameof(recursive);
                var deleteEmptyString = (deleteEmptyDirectories ? string.Empty : "don't ") + nameof(deleteEmptyDirectories);
                log.Trace(() => Invariant($"Started cleaning-up the directory {rootPath}, {retentionWindow}, {dateRetrievalStrategy}, {recursiveString}, {deleteEmptyString}."));

                if (!File.GetAttributes(rootPath).HasFlag(FileAttributes.Directory))
                {
                    throw new ArgumentException("Root path must be a directory.");
                }

                if (!Directory.Exists(rootPath))
                {
                    throw new ArgumentException("Root path: " + rootPath + " does not exist.");
                }

                var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                log.Trace(() => "Identifying all files that should be considered for cleanup.");
                var files = Directory.GetFiles(rootPath, "*", searchOptions);

                log.Trace(() => "Filtering to files that are outside the retention window.");
                var cutoff = DateTime.UtcNow.Subtract(retentionWindow);
                var filesToDelete = FilterFilesToBeforeCutOff(files, cutoff, dateRetrievalStrategy);

                foreach (var fileToDelete in filesToDelete)
                {
                    var localFileToDelete = fileToDelete;
                    log.Trace(
                        () =>
                        "File: " + localFileToDelete + " is being removed because it's outside of the retention window.");
                    File.Delete(fileToDelete);
                }

                if (deleteEmptyDirectories)
                {
                    log.Trace(() => "Removing any empty directories.");
                    foreach (
                        var directoryPath in Directory.GetDirectories(rootPath, "*", searchOptions))
                    {
                        var directory = new DirectoryInfo(directoryPath);
                        if (!directory.GetFiles().Any())
                        {
                            var localDirectoryPath = directoryPath;
                            log.Trace(
                                () => "Directory: " + localDirectoryPath + " is being removed because it's empty.");
                            directory.Delete(recursive);
                        }
                    }
                }

                log.Trace(() => "Completed cleaning-up the directory.");
            }
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
