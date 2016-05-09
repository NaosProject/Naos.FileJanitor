// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreFileInS3MessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Its.Configuration;
    using Its.Log.Instrumentation;

    using Naos.AWS.Core;
    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.Domain;

    /// <summary>
    /// Message handler to store files in S3.
    /// </summary>
    public class StoreFileInS3MessageHandler : IHandleMessages<StoreFileInS3Message>, IShareFilePath
    {
        /// <inheritdoc />
        public async Task HandleAsync(StoreFileInS3Message message)
        {
            if (message.FilePath == null || !File.Exists(message.FilePath))
            {
                throw new FileNotFoundException("Could not find specified filepath: " + (message.FilePath ?? "[NULL]"));
            }

            if (string.IsNullOrEmpty(message.BucketName))
            {
                throw new ApplicationException("Must specify bucket name.");
            }

            var settings = Settings.Get<FileJanitorMessageHandlerSettings>();
            await this.HandleAsync(message, settings);
        }

        /// <summary>
        /// Handles a StoreFileInS3Message.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        /// <returns>Task to support async await execution.</returns>
        public async Task HandleAsync(StoreFileInS3Message message, FileJanitorMessageHandlerSettings settings)
        {
            using (var log = Log.Enter(() => new { Message = message, message.Region, message.BucketName, message.Key, message.FilePath }))
            {
                log.Trace(() => "Starting upload.");
                var fileManager = new FileManager(settings.UploadAccessKey, settings.UploadSecretKey);
                this.FilePath = message.FilePath;
                await fileManager.UploadFileAsync(message.Region, message.BucketName, message.Key, this.FilePath);
                log.Trace(() => "Finished upload.");
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
