// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleKeysFoundStrategy.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    /// <summary>
    /// Enumeration of the ways to handle multiple keys found in a container.
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
        FirstSortedDescending,
    }
}