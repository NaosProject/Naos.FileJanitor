// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareFileLocationMessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.Threading.Tasks;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.Domain;

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
