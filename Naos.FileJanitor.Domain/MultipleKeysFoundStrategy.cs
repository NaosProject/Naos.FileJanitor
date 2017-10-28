// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleKeysFoundStrategy.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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