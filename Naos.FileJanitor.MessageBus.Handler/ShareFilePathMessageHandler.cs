// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareFilePathMessageHandler.cs" company="Naos">
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
