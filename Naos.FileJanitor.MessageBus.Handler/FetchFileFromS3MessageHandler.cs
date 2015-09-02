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

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;

    using Its.Configuration;
    using Its.Log.Instrumentation;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.HandlingContract;

    /// <summary>
    /// Message handler to fetch a file from S3.
    /// </summary>
    public class FetchFileFromS3MessageHandler : IHandleMessages<FetchFileFromS3Message>, IShareFilePath
    {
        /// <inheritdoc />
        public void Handle(FetchFileFromS3Message message)
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
            this.Handle(message, settings);
        }

        /// <summary>
        /// Handles a FetchFileFromS3Message.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        public void Handle(FetchFileFromS3Message message, FileJanitorMessageHandlerSettings settings)
        {
            using (var log = Log.Enter(() => message))
            {
                var regionEndpoint = RegionEndpoint.GetBySystemName(message.Region);
                using (
                    var client = new AmazonS3Client(
                        settings.DownloadAccessKey,
                        settings.DownloadSecretKey,
                        regionEndpoint))
                {
                    var request = new ListObjectsRequest
                                      {
                                          BucketName = message.BucketName,
                                          Prefix = message.KeyPrefixSearchPattern
                                      };

                    var objects = client.ListObjects(request);
                    if (message.MultipleKeysFoundStrategy == MultipleKeysFoundStrategy.SingleMatchExpectedThrow
                        && objects.S3Objects.Count > 1)
                    {
                        throw new InvalidDataException(
                            "Expected a single S3Object => Prefix Search: "
                            + (message.KeyPrefixSearchPattern ?? "[NULL]") + ", Count: " + objects.S3Objects.Count);
                    }

                    var keys = objects.S3Objects.Select(_ => _.Key).ToList();
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
                            "Could not find an S3 Object => bucket: " + message.BucketName
                            + ", KeyPrefixSearchPattern: " + message.KeyPrefixSearchPattern);
                    }

                    using (var transferUtility = new TransferUtility(client))
                    {
                        this.FilePath = message.FilePath.Replace("{Key}", key);
                        log.Trace(
                            () =>
                            "Dowloading the file from the specified bucket => key: " + key + " filePath: "
                            + this.FilePath);
                        transferUtility.Download(this.FilePath, message.BucketName, key);
                    }
                }

                log.Trace(() => "Completed downloading the file from the specified bucket");
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
