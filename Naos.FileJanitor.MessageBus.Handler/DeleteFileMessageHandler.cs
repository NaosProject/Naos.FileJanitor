// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteFileMessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System.IO;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.HandlingContract;

    /// <summary>
    /// Message handler to delete a file.
    /// </summary>
    public class DeleteFileMessageHandler : IHandleMessages<DeleteFileMessage>, IShareFilePath
    {
        /// <inheritdoc />
        public void Handle(DeleteFileMessage message)
        {
            using (var log = Log.Enter(() => message))
            {
                if (message.FilePath == null || !File.Exists(message.FilePath))
                {
                    throw new FileNotFoundException("Could not find specified filepath: " + (message.FilePath ?? "[NULL]"));
                }

                log.Trace(() => "Deleting this file: " + message.FilePath);
                File.Delete(message.FilePath);

                this.FilePath = message.FilePath;
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
