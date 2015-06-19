// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteFileMessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handlers
{
    using System.IO;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.HandlingContract;

    /// <summary>
    /// Message handler to store files in S3.
    /// </summary>
    public class DeleteFileMessageHandler : IHandleMessages<DeleteFileMessage>
    {
        /// <inheritdoc />
        public void Handle(DeleteFileMessage message)
        {
            if (message.FilePath == null || !File.Exists(message.FilePath))
            {
                throw new FileNotFoundException("Could not find specified filepath: " + (message.FilePath ?? "[NULL]"));
            }

            File.Delete(message.FilePath);
        }
    }
}
