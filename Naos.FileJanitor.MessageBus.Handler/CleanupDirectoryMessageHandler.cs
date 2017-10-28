// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanupDirectoryMessageHandler.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.Domain;
    using Naos.FileJanitor.MessageBus.Scheduler;
    using Naos.MessageBus.Domain;

    /// <summary>
    /// Handler to handle CleanupDirectoryMessages.
    /// </summary>
    public class CleanupDirectoryMessageHandler : MessageHandlerBase<CleanupDirectoryMessage>
    {
        /// <inheritdoc cref="MessageHandlerBase{T}" />
        public override async Task HandleAsync(CleanupDirectoryMessage message)
        {
            await Task.Run(() => InternalHandle(message));
        }

        private static void InternalHandle(CleanupDirectoryMessage message)
        {
            using (var log = Log.Enter(() => message))
            {
                log.Trace(() => "Started cleaning-up the directory.");

                if (!File.GetAttributes(message.DirectoryFullPath).HasFlag(FileAttributes.Directory))
                {
                    throw new ArgumentException("Root path must be a directory.");
                }

                if (!Directory.Exists(message.DirectoryFullPath))
                {
                    throw new ArgumentException("Root path: " + message.DirectoryFullPath + " does not exist.");
                }

                var searchOptions = message.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                log.Trace(() => "Identifying all files that should be considered for cleanup.");
                var files = Directory.GetFiles(message.DirectoryFullPath, "*", searchOptions);

                log.Trace(() => "Filtering to files that are outside the retention window.");
                var cutoff = DateTime.UtcNow.Subtract(message.RetentionWindow);
                var filesToDelete = FilterFilesToBeforeCutOff(files, cutoff, message.FileDateRetrievalStrategy);

                foreach (var fileToDelete in filesToDelete)
                {
                    var localFileToDelete = fileToDelete;
                    log.Trace(
                        () =>
                        "File: " + localFileToDelete + " is being removed because it's outside of the retention window.");
                    File.Delete(fileToDelete);
                }

                if (message.DeleteEmptyDirectories)
                {
                    log.Trace(() => "Removing any empty directories.");
                    foreach (
                        var directoryPath in Directory.GetDirectories(message.DirectoryFullPath, "*", searchOptions))
                    {
                        var directory = new DirectoryInfo(directoryPath);
                        if (!directory.GetFiles().Any())
                        {
                            var localDirectoryPath = directoryPath;
                            log.Trace(
                                () => "Directory: " + localDirectoryPath + " is being removed because it's empty.");
                            directory.Delete(message.Recursive);
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
