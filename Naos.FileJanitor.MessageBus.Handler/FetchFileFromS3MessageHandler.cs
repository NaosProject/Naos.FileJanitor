// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FetchFileFromS3MessageHandler.cs" company="Naos">
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

    using Naos.AWS.S3;
    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.Domain;

    using Spritely.Redo;

    /// <summary>
    /// Message handler to fetch a file from S3.
    /// </summary>
    public class FetchFileFromS3MessageHandler : IHandleMessages<FetchFileMessage>, IShareFilePath, IShareAffectedItems
    {
        /// <inheritdoc />
        public async Task HandleAsync(FetchFileMessage message)
        {
            if (message.FilePath == null)
            {
                throw new FileNotFoundException("Could not use specified filepath: " + (message.FilePath ?? "[NULL]"));
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
        /// Handles a FetchFileFromS3Message.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        /// <returns>Task to support async await execution.</returns>
        public async Task HandleAsync(FetchFileMessage message, FileJanitorMessageHandlerSettings settings)
        {
            var correlationId = Guid.NewGuid().ToString().ToUpperInvariant();
            Log.Write(() => $"Starting Fetch File; CorrelationId: { correlationId }, Region: {message.FileLocation.ContainerLocation}, BucketName: {message.FileLocation.Container}, Key: {message.FileLocation.Key}, RawFilePath: {message.FilePath}");
            using (var log = Log.Enter(() => new { CorrelationId = correlationId }))
            {
                var fileManager = new FileManager(settings.DownloadAccessKey, settings.DownloadSecretKey);

                // shares path down because it can be augmented...
                this.FilePath = message.FilePath.Replace("{Key}", message.FileLocation.Key);
                log.Trace(() => $"Dowloading the file to replaced FilePath: {this.FilePath}");

                await
                    Using.LinearBackOff(TimeSpan.FromSeconds(5))
                        .WithMaxRetries(3)
                        .RunAsync(
                            () =>
                                fileManager.DownloadFileAsync(
                                    message.FileLocation.ContainerLocation,
                                    message.FileLocation.Container,
                                    message.FileLocation.Key,
                                    this.FilePath))
                        .Now();

                var affectedItem = new FileLocationAffectedItem
                                       {
                                           FileLocationAffectedItemMessage = "Fetched file from location to path.",
                                           FileLocation = message.FileLocation,
                                           FilePath = this.FilePath
                                       };

                this.AffectedItems = new[] { new AffectedItem { Id = affectedItem.ToJson() } };
                log.Trace(() => "Completed downloading the file");
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }

        /// <inheritdoc />
        public AffectedItem[] AffectedItems { get; set; }
    }
}
