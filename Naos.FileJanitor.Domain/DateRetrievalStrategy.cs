// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateRetrievalStrategy.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
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
        LastAccessDate,
    }
}
