// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShareFilePath.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using Naos.MessageBus.DataContract;

    /// <summary>
    /// Interface to support sharing a file path between handlers and future messages.
    /// </summary>
    public interface IShareFilePath : IShare
    {
        /// <summary>
        /// Gets or sets the file path to share.
        /// </summary>
        string FilePath { get; set; }
    }
}
