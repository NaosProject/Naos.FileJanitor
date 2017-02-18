// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoreFileMessage.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using System.Collections.Generic;
    using System.Security.Cryptography;

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

        /// <summary>
        /// Gets or sets the hashing algorithms to compute, persist, and use in verification.
        /// </summary>
        public IReadOnlyCollection<HashAlgorithmName> HashingAlgorithms { get; set; }

        /// <summary>
        /// Gets or sets user defined meta data to save with the file.
        /// </summary>
        public IReadOnlyDictionary<string, string> UserDefinedMetadata { get; set; }
    }
}
