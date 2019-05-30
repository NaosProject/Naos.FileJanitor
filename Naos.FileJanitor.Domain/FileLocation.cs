// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLocation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    using System;

    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// Model to represent a specific file.
    /// </summary>
    public class FileLocation : IEquatable<FileLocation>
    {
        /// <summary>
        /// Gets or sets the storage location.
        /// </summary>
        public string ContainerLocation { get; set; }

        /// <summary>
        /// Gets or sets the storage container.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets the key of the file.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(FileLocation first, FileLocation second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return string.Equals(first.ContainerLocation, second.ContainerLocation, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(first.Container, second.Container, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(first.Key, second.Key, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are unequal.</returns>
        public static bool operator !=(FileLocation first, FileLocation second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(FileLocation other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as FileLocation);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContainerLocation).Hash(this.Container).Hash(this.Key).Value;
    }
}