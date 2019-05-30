// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataItem.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Model object to hold a metadata entry and allowing sharing a collection of them where <see cref="IReadOnlyDictionary{TKey,TValue}" /> cannot be used.
    /// </summary>
    public class MetadataItem : IEquatable<MetadataItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataItem"/> class.
        /// </summary>
        /// <param name="key">Key value.</param>
        /// <param name="value">Value value.</param>
        public MetadataItem(string key, string value)
        {
            new { key }.Must().NotBeNull();

            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the metadata key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(MetadataItem first, MetadataItem second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return string.Equals(first.Key, second.Key, StringComparison.OrdinalIgnoreCase) && 
                   string.Equals(first.Value, second.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are unequal.</returns>
        public static bool operator !=(MetadataItem first, MetadataItem second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(MetadataItem other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as MetadataItem);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.Key)
            .Hash(this.Value).Value;
    }

    /// <summary>
    /// Extensions for converting metadata items in and out of a <see cref="IReadOnlyDictionary{TKey,TValue}" />.
    /// </summary>
    public static class MetadataItemExtensions
    {
        /// <summary>
        /// Convert an collection of metadata items into a <see cref="IReadOnlyDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="metadataItems">Items to convert.</param>
        /// <returns>Converted collection.</returns>
        public static IReadOnlyDictionary<string, string> ToReadOnlyDictionary(this IReadOnlyCollection<MetadataItem> metadataItems)
        {
            var ret = metadataItems.ToDictionary(k => k.Key, v => v.Value);
            return ret;
        }

        /// <summary>
        /// Convert an dictionary of metadata items into a <see cref="IReadOnlyCollection{TValue}" />.
        /// </summary>
        /// <param name="metadata">Items to convert.</param>
        /// <returns>Converted collection.</returns>
        public static IReadOnlyCollection<MetadataItem> ToReadOnlyCollection(this IReadOnlyDictionary<string, string> metadata)
        {
            var ret = metadata.Select(_ => new MetadataItem(_.Key, _.Value)).ToList();
            return ret;
        }
    }
}
