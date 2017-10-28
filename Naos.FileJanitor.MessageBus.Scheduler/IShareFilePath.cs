// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShareFilePath.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Scheduler
{
    using Naos.MessageBus.Domain;

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
