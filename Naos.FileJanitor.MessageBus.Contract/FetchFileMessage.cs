// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FetchFileMessage.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using Naos.MessageBus.Domain;

    /// <summary>
    /// Message to get a specified file from storage.
    /// </summary>
    public class FetchFileMessage : IMessage, IShareFileLocation, IShareFilePath
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the source location.
        /// </summary>
        public FileLocation FileLocation { get; set; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        public string FilePath { get; set; }
    }
}
