// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLocationAffectedItem.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using Naos.FileJanitor.MessageBus.Contract;

    /// <summary>
    /// Model object to hold an affected item from .
    /// </summary>
    public class FileLocationAffectedItem
    {
        /// <summary>
        /// Gets or sets a message about the event.
        /// </summary>
        public string FileLocationAffectedItemMessage { get; set; }

        /// <summary>
        /// Gets or sets the source or target location.
        /// </summary>
        public FileLocation FileLocation { get; set; }

        /// <summary>
        /// Gets or sets the source or target path.
        /// </summary>
        public string FilePath { get; set; }
    }
}
