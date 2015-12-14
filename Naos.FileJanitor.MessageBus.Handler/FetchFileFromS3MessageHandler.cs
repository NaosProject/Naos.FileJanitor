// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FetchFileFromS3MessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Its.Configuration;
    using Its.Log.Instrumentation;

    using Naos.AWS.Core;
    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.HandlingContract;

    /// <summary>
    /// Message handler to fetch a file from S3.
    /// </summary>
    public class FetchFileFromS3MessageHandler : IHandleMessages<FetchFileFromS3Message>, IShareFilePath
    {
        /// <inheritdoc />
        public async Task HandleAsync(FetchFileFromS3Message message)
        {
            if (message.FilePath == null)
            {
                throw new FileNotFoundException("Could not use specified filepath: " + (message.FilePath ?? "[NULL]"));
            }

            if (string.IsNullOrEmpty(message.BucketName))
            {
                throw new ApplicationException("Must specify bucket name.");
            }

            var settings = Settings.Get<FileJanitorMessageHandlerSettings>();
            await this.HandleAsync(message, settings);
        }

        /// <summary>
        /// Handles a FetchFileFromS3Message.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        /// <returns>Task to support async await execution.</returns>
        public async Task HandleAsync(FetchFileFromS3Message message, FileJanitorMessageHandlerSettings settings)
        {
            using (var log = Log.Enter(() => message))
            {
                var fileManager = new FileManager(settings.DownloadAccessKey, settings.DownloadSecretKey);

                var files = await fileManager.ListFilesAsync(message.Region, message.BucketName, message.KeyPrefixSearchPattern);
                if (message.MultipleKeysFoundStrategy == MultipleKeysFoundStrategy.SingleMatchExpectedThrow
                    && files.Count > 1)
                {
                    throw new InvalidDataException(
                        "Expected a single S3Object => Prefix Search: " + (message.KeyPrefixSearchPattern ?? "[NULL]")
                        + ", Count: " + files.Count);
                }

                var keys = files.Select(_ => _.KeyName).ToList();
                switch (message.MultipleKeysFoundStrategy)
                {
                    case MultipleKeysFoundStrategy.FirstSortedAscending:
                        keys = keys.OrderBy(_ => _).ToList();
                        break;
                    case MultipleKeysFoundStrategy.FirstSortedDescending:
                        keys = keys.OrderByDescending(_ => _).ToList();
                        break;
                    default:
                        throw new NotSupportedException(
                            "Unsupported multiple found strategy => " + message.MultipleKeysFoundStrategy);
                }

                var key = keys.FirstOrDefault();
                if (key == null)
                {
                    throw new FileNotFoundException(
                        "Could not find an S3 Object => bucket: " + message.BucketName + ", KeyPrefixSearchPattern: "
                        + message.KeyPrefixSearchPattern);
                }

                this.FilePath = message.FilePath.Replace("{Key}", key);
                Log.Write(
                    () => "Dowloading the file from the specified bucket => key: " + key + " filePath: " + this.FilePath);

                await fileManager.DownloadFileAsync(message.Region, message.BucketName, key, this.FilePath);

                Log.Write(() => "Completed downloading the file from the specified bucket");
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
