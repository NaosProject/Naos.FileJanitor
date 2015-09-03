// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteFileMessageHandler.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System.IO;
    using System.Threading.Tasks;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.MessageBus.HandlingContract;

    /// <summary>
    /// Message handler to delete a file.
    /// </summary>
    public class DeleteFileMessageHandler : IHandleMessages<DeleteFileMessage>, IShareFilePath
    {
        /// <inheritdoc />
        public async Task Handle(DeleteFileMessage message)
        {
            using (var log = Log.Enter(() => new { Message = message, message.FilePath }))
            {
                if (message.FilePath == null || !File.Exists(message.FilePath))
                {
                    throw new FileNotFoundException(
                        "Could not find specified filepath: " + (message.FilePath ?? "[NULL]"));
                }

                this.FilePath = message.FilePath;

                log.Trace(() => "Start deleting file.");
                await Task.Run(() => File.Delete(message.FilePath));
                log.Trace(() => "Finished deleting file.");
            }
        }

        /// <inheritdoc />
        public string FilePath { get; set; }
    }
}
