// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FetchFileFromS3Message.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using Naos.MessageBus.DataContract;

    /// <summary>
    /// Message to get a file from S3.
    /// </summary>
    public class FetchFileFromS3Message : IMessage, IShareFilePath
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path (in the context of the handling of the message) of file downloaded from S3.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the region the intended bucket lives in.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets bucket name to look for the file in.
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the search pattern for the key (multiples will be handled according to the strategy).
        /// </summary>
        public string KeyPrefixSearchPattern { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use when multiple keys are found (default is throw on multiples).
        /// </summary>
        public MultipleKeysFoundStrategy MultipleKeysFoundStrategy { get; set; }
    }
}
