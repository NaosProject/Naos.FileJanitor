// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareFileLocationMessage.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using System;
    using System.Threading.Tasks;

    using Its.Log.Instrumentation;

    using Naos.MessageBus.Domain;

    /// <summary>
    /// Message object to share a file path with remaining messages.
    /// </summary>
    public class ShareFileLocationMessage : IMessage
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the location (in the context of the handling of the message) of file to share with rest of sequence.
        /// </summary>
        public FileLocation FileLocationToShare { get; set; }
    }

    /// <summary>
    /// Message handler for <see cref="ShareFileLocationMessage"/>.
    /// </summary>
    public class ShareFileLocationMessageHandler : IHandleMessages<ShareFileLocationMessage>, IShareFileLocation
    {
        /// <inheritdoc />
        public async Task HandleAsync(ShareFileLocationMessage message)
        {
            var correlationId = Guid.NewGuid().ToString().ToUpperInvariant();
            Log.Write(() => $"Sharing file location; CorrelationId: { correlationId }, container location: {message.FileLocationToShare.ContainerLocation}, container: {message.FileLocationToShare.Container}, key: {message.FileLocationToShare.Key}");
            using (var log = Log.Enter(() => new { CorrelationId = correlationId }))
            {
                log.Trace(() => $"Sharing file location.");
                this.FileLocation = await Task.FromResult(message.FileLocationToShare);
                log.Trace(() => $"Shared file location.");
            }
        }

        /// <inheritdoc />
        public FileLocation FileLocation { get; set; }
    }
}
