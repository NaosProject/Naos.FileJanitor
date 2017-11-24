// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreFileInS3MessageHandler.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using ByteSizeLib;

    using Its.Configuration;
    using Its.Log.Instrumentation;

    using Naos.AWS.S3;
    using Naos.FileJanitor.Domain;
    using Naos.FileJanitor.MessageBus.Scheduler;
    using Naos.MessageBus.Domain;

    using Spritely.Redo;

    /// <summary>
    /// Message handler to store files in S3.
    /// </summary>
    public class StoreFileInS3MessageHandler : MessageHandlerBase<StoreFileMessage>, IShareAffectedItems
    {
        /// <inheritdoc cref="MessageHandlerBase{T}" />
        public override async Task HandleAsync(StoreFileMessage message)
        {
            if (message.FilePath == null || !File.Exists(message.FilePath))
            {
                throw new FileNotFoundException("Could not find specified filepath: " + (message.FilePath ?? "[NULL]"));
            }

            if (message.FileLocation == null)
            {
                throw new ApplicationException("Must specify file location to fetch from.");
            }

            if (string.IsNullOrEmpty(message.FileLocation.ContainerLocation))
            {
                throw new ApplicationException("Must specify region (container location).");
            }

            if (string.IsNullOrEmpty(message.FileLocation.Container))
            {
                throw new ApplicationException("Must specify bucket name (container).");
            }

            var settings = Settings.Get<FileJanitorMessageHandlerSettings>();
            await this.HandleAsync(message, settings);
        }

        /// <summary>
        /// Handles a <see cref="StoreFileMessage"/>.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        /// <returns>Task to support async await execution.</returns>
        public async Task HandleAsync(StoreFileMessage message, FileJanitorMessageHandlerSettings settings)
        {
            var correlationId = Guid.NewGuid().ToString().ToUpperInvariant();
            Log.Write(() => $"Starting Store File; CorrelationId: {correlationId}, Region: {message.FileLocation.ContainerLocation}, BucketName: {message.FileLocation.Container}, Key: {message.FileLocation.Key}, FilePath: {message.FilePath}");
            using (var log = Log.Enter(() => new { CorrelationId = correlationId }))
            {
                log.Trace(() => "Starting upload.");

                var fileManager = new FileManager(settings.UploadAccessKey, settings.UploadSecretKey);

                var fileSize = ByteSize.FromBytes(new FileInfo(message.FilePath).Length);
                var attemptWaitTimeMultiplier = TimeSpan.FromSeconds(fileSize.MegaBytes * 0.001);
                var minimumAttemptWaitTimeMultiplier = TimeSpan.FromSeconds(5);
                if (attemptWaitTimeMultiplier < minimumAttemptWaitTimeMultiplier)
                {
                    attemptWaitTimeMultiplier = minimumAttemptWaitTimeMultiplier;
                }

                await
                    Using.LinearBackOff(attemptWaitTimeMultiplier)
                        .WithMaxRetries(3)
                        .RunAsync(
                            () =>
                                fileManager.UploadFileAsync(
                                    message.FileLocation.ContainerLocation,
                                    message.FileLocation.Container,
                                    message.FileLocation.Key ?? Path.GetFileName(message.FilePath),
                                    message.FilePath,
                                    message.HashingAlgorithms ?? new HashAlgorithmName[0],
                                    (message.UserDefinedMetadata ?? new MetadataItem[0]).ToReadOnlyDictionary()))
                        .Now();

                var affectedItem = new FileLocationAffectedItem
                {
                    FileLocationAffectedItemMessage = "Stored file from path to location.",
                    FileLocation = message.FileLocation,
                    FilePath = message.FilePath,
                };

                var serializer = this.SerializerFactory.BuildSerializer(FileLocationAffectedItem.ItemSerializationDescription);

                this.AffectedItems = new[] { new AffectedItem { Id = serializer.SerializeToString(affectedItem) } };

                log.Trace(() => "Finished upload.");
            }
        }

        /// <inheritdoc />
        public AffectedItem[] AffectedItems { get; set; }
    }
}
