// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShareFileLocation.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Scheduler
{
    using Naos.FileJanitor.Domain;
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
