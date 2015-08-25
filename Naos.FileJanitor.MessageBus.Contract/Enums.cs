// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    /// <summary>
    /// Enumeration of the ways a files date can be retrieved.
    /// </summary>
    public enum DateRetrievalStrategy
    {
        /// <summary>
        /// Use the create date of the item when evaluating.
        /// </summary>
        CreateDate,

        /// <summary>
        /// Use the last update date of the item when evaluating.
        /// </summary>
        LastUpdateDate,

        /// <summary>
        /// Use the last access date of the item when evaluating.
        /// </summary>
        LastAccessDate
    }

    /// <summary>
    /// Enumeration of the ways to handle multiple keys found in an S3 bucket.
    /// </summary>
    public enum MultipleKeysFoundStrategy
    {
        /// <summary>
        /// A single match is expected so throw an exception.
        /// </summary>
        SingleMatchExpectedThrow,

        /// <summary>
        /// Sort the collection ascending by key and choose first.
        /// </summary>
        FirstSortedAscending,

        /// <summary>
        /// Sort the collection descending by key and choose first.
        /// </summary>
        FirstSortedDescending
    }
}
