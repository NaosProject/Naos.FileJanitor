﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreFileInS3MessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handlers
{
    using System.Collections.Generic;
    using System.IO;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Transfer;

    using Its.Configuration;

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
            if (message.BucketName != null && message.FilePath != null && File.Exists(message.FilePath))
            {
                var settings = Settings.Get<FileJanitorMessageHandlerSettings>();
                this.Handle(message, settings);
            }
        }

        /// <summary>
        /// Handles a StoreFileInS3Message.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        public void Handle(StoreFileInS3Message message, FileJanitorMessageHandlerSettings settings)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(message.Region);
            var client = new AmazonS3Client(settings.UploadAccessKey, settings.UploadSecretKey, regionEndpoint);
            var transferUtility = new TransferUtility(client);
            transferUtility.Upload(message.FilePath, message.BucketName);

            this.FilePath = message.FilePath;
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}