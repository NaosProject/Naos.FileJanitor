// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareFileLocationMessage.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
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
}
