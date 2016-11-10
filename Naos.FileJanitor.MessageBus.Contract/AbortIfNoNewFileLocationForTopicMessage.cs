// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbortIfNoNewFileLocationForTopicMessage.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using Naos.MessageBus.Domain;

    /// <summary>
    /// Message to abort if a provided file location is the same as the one in the affected items.
    /// </summary>
    public class AbortIfNoNewFileLocationForTopicMessage : IMessage, IShareFileLocation, IShareTopicStatusReports
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the topic to check.
        /// </summary>
        public NamedTopic TopicToCheckAffectedItemsFor { get; set; }

        /// <inheritdoc />
        public FileLocation FileLocation { get; set; }

        /// <inheritdoc />
        public TopicStatusReport[] TopicStatusReports { get; set; }
    }
}
