// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanupDirectoryMessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.HandlingContract;

    /// <summary>
    /// Handler to handle CleanupDirectoryMessages.
    /// </summary>
    public class CleanupDirectoryMessageHandler : IHandleMessages<CleanupDirectoryMessage>
    {
        /// <inheritdoc />
        public void Handle(CleanupDirectoryMessage message)
        {
            if (!File.GetAttributes(message.DirectoryFullPath).HasFlag(FileAttributes.Directory))
            {
                throw new ArgumentException("Root path must be a directory.");
            }

            if (!Directory.Exists(message.DirectoryFullPath))
            {
                throw new ArgumentException("Root path: " + message.DirectoryFullPath + " does not exist.");
            }

            var searchOptions = message.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var files = Directory.GetFiles(
                message.DirectoryFullPath,
                "*",
                searchOptions);

            var cutoff = DateTime.UtcNow.Subtract(message.RetentionWindow);
            var filesToDelete = FilterFilesToBeforeCutOff(files, cutoff, message.FileDateRetrievalStrategy);

            foreach (var fileToDelete in filesToDelete)
            {
                Console.WriteLine("File: " + fileToDelete + " is being removed because it's outside of the retention window.");
                File.Delete(fileToDelete);
            }

            if (message.DeleteEmptyDirectories)
            {
                foreach (var directoryPath in Directory.GetDirectories(message.DirectoryFullPath, "*", searchOptions))
                {
                    var directory = new DirectoryInfo(directoryPath);
                    if (!directory.GetFiles().Any())
                    {
                        Console.WriteLine("Directory: " + directoryPath + " is being removed because it's empty.");
                        directory.Delete(message.Recursive);
                    }
                }
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
                        throw new ArgumentException("Unsupported DateRetreivalStrategy: " + dateRetrievalStrategy);
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
