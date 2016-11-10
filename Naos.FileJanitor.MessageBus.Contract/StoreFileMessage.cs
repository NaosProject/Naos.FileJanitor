// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreFileMessage.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using Naos.MessageBus.Domain;

    /// <summary>
    /// Message object to store a file in centralized storage.
    /// </summary>
    public class StoreFileMessage : IMessage, IShareFilePath, IShareFileLocation
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the target location.
        /// </summary>
        public FileLocation FileLocation { get; set; }
    }
}
