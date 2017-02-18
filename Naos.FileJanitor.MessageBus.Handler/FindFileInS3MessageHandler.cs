// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindFileInS3MessageHandler.cs" company="Naos">
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

    using Naos.AWS.S3;
    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.Domain;

    using Spritely.Redo;

    /// <summary>
    /// Message handler to fetch a file from S3.
    /// </summary>
    public class FindFileInS3MessageHandler : IHandleMessages<FindFileMessage>, IShareFileLocation
    {
        /// <inheritdoc />
        public async Task HandleAsync(FindFileMessage message)
        {
            if (string.IsNullOrEmpty(message.ContainerLocation))
            {
                throw new ApplicationException("Must specify region (container location).");
            }

            if (string.IsNullOrEmpty(message.Container))
            {
                throw new ApplicationException("Must specify bucket name (container).");
            }

            var settings = Settings.Get<FileJanitorMessageHandlerSettings>();
            await this.HandleAsync(message, settings);
        }

        /// <summary>
        /// Handles a <see cref="FindFileMessage"/>.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="settings">Needed settings to handle messages.</param>
        /// <returns>Task to support async await execution.</returns>
        public async Task HandleAsync(FindFileMessage message, FileJanitorMessageHandlerSettings settings)
        {
            var correlationId = Guid.NewGuid().ToString().ToUpperInvariant();
            Log.Write(() => $"Starting Find File; CorrelationId: { correlationId }, ContainerLocation/Region: {message.ContainerLocation}, Container/BucketName: {message.Container}, KeyPrefixSearchPattern: {message.KeyPrefixSearchPattern}, MultipleKeysFoundStrategy: {message.MultipleKeysFoundStrategy}");
            using (var log = Log.Enter(() => new { CorrelationId = correlationId }))
            {
                var fileManager = new FileManager(settings.DownloadAccessKey, settings.DownloadSecretKey);

                var files =
                    await
                        Using.LinearBackOff(TimeSpan.FromSeconds(5))
                            .WithMaxRetries(3)
                            .Run(() => fileManager.ListFilesAsync(message.ContainerLocation, message.Container, message.KeyPrefixSearchPattern))
                            .Now();

                if (message.MultipleKeysFoundStrategy == MultipleKeysFoundStrategy.SingleMatchExpectedThrow && files.Count > 1)
                {
                    throw new InvalidDataException(
                              "Expected a single S3Object => Prefix Search: " + (message.KeyPrefixSearchPattern ?? "[NULL]") + ", Count: " + files.Count);
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
                        throw new NotSupportedException("Unsupported multiple found strategy => " + message.MultipleKeysFoundStrategy);
                }

                var key = keys.FirstOrDefault();
                if (key == null)
                {
                    throw new FileNotFoundException(
                        $"Could not find an S3 Object => region: {message.ContainerLocation}, bucket: {message.Container}, KeyPrefixSearchPattern: {message.KeyPrefixSearchPattern}");
                }

                // share the results
                this.FileLocation = new FileLocation { ContainerLocation = message.ContainerLocation, Container = message.Container, Key = key };

                log.Trace(() => "Completed finding file");
            }
        }

        /// <inheritdoc />
        public FileLocation FileLocation { get; set; }
    }
}
