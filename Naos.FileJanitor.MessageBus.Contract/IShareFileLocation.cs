// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShareFileLocation.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using Naos.MessageBus.Domain;

    /// <summary>
    /// Interface to support sharing a file location between handlers and future messages.
    /// </summary>
    public interface IShareFileLocation : IShare
    {
        /// <summary>
        /// Gets or sets details about a file.
        /// </summary>
        FileLocation FileLocation { get; set; }
    }
}
