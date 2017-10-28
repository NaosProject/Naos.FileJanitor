// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareFileLocationMessage.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Scheduler
{
    using System.Threading.Tasks;

    using Naos.FileJanitor.Domain;
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
    public class ShareFileLocationMessageHandler : MessageHandlerBase<ShareFileLocationMessage>, IShareFileLocation
    {
        /// <inheritdoc cref="MessageHandlerBase{T}" />
        public override async Task HandleAsync(ShareFileLocationMessage message)
        {
            this.FileLocation = await Task.FromResult(message.FileLocationToShare);
        }

        /// <inheritdoc />
        public FileLocation FileLocation { get; set; }
    }
}
