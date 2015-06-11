// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreFileInS3Message.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using Naos.MessageBus.DataContract;

    /// <summary>
    /// Message object to put a file into S3.
    /// </summary>
    public class StoreFileInS3Message : IMessage
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path (in the context of the handling of the message) of file to put in S3.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets bucket name to store the file in.
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the region the intended bucket lives in.
        /// </summary>
        public string Region { get; set; }
    }
}
