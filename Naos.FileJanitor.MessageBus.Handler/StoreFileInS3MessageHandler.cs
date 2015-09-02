// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreFileInS3MessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.IO;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Transfer;

    using Its.Configuration;
    using Its.Log.Instrumentation;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.HandlingContract;

    /// <summary>
    /// Message handler to store files in S3.
    /// </summary>
    public class StoreFileInS3MessageHandler : IHandleMessages<StoreFileInS3Message>, IShareFilePath
    {
        /// <inheritdoc />
        public void Handle(StoreFileInS3Message message)
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
            this.Handle(message, settings);
        }

        /// <summary>
        /// Handles a StoreFileInS3Message.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        public void Handle(StoreFileInS3Message message, FileJanitorMessageHandlerSettings settings)
        {
            using (var log = Log.Enter(() => message))
            {
                var regionEndpoint = RegionEndpoint.GetBySystemName(message.Region);
                using (var client = new AmazonS3Client(settings.UploadAccessKey, settings.UploadSecretKey, regionEndpoint))
                {
                    using (var transferUtility = new TransferUtility(client))
                    {
                        log.Trace(() => "Uploading the file to the specified bucket");
                        transferUtility.Upload(message.FilePath, message.BucketName, message.Key);

                        log.Trace(() => "Completed uploading the file to the specified bucket");
                        this.FilePath = message.FilePath;
                    }
                }
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
