// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareFilePathMessage.cs" company="Naos">
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
    public class ShareFilePathMessage : IMessage
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path (in the context of the handling of the message) of file to share with rest of sequence.
        /// </summary>
        public string FilePathToShare { get; set; }
    }
    
    /// <summary>
    /// Message handler for <see cref="ShareFilePathMessage"/>.
    /// </summary>
    public class ShareFilePathMessageHandler : IHandleMessages<ShareFilePathMessage>, IShareFilePath
    {
        /// <inheritdoc />
        public async Task HandleAsync(ShareFilePathMessage message)
        {
            var correlationId = Guid.NewGuid().ToString().ToUpperInvariant();
            Log.Write(() => $"Sharing file path; CorrelationId: {correlationId}, FilePathToShare: {message.FilePathToShare}");
            using (var log = Log.Enter(() => new { CorrelationId = correlationId }))
            {
                log.Trace(() => "Sharing file path.");
                this.FilePath = await Task.FromResult(message.FilePathToShare);
                log.Trace(() => "Shared file path.");
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
