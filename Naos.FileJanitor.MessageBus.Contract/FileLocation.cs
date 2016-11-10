// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLocation.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Contract
{
    using System;

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

        #region Equality

        /// <inheritdoc />
        public static bool operator ==(FileLocation keyObject1, FileLocation keyObject2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(keyObject1, keyObject2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)keyObject1 == null) || ((object)keyObject2 == null))
            {
                return false;
            }

            return keyObject1.Equals(keyObject2);
        }

        /// <inheritdoc />
        public static bool operator !=(FileLocation keyObject1, FileLocation keyObject2)
        {
            return !(keyObject1 == keyObject2);
        }

        /// <inheritdoc />
        public bool Equals(FileLocation other)
        {
            if (other == null)
            {
                return false;
            }

            var result = (this.ContainerLocation == other.ContainerLocation) && (this.Container == other.Container) && (this.Key == other.Key);
            return result;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var keyObject = obj as FileLocation;
            if (keyObject == null)
            {
                return false;
            }

            return this.Equals(keyObject);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ this.ContainerLocation.GetHashCode();
                hash = hash * 16777619 ^ this.Container.GetHashCode();
                hash = hash * 16777619 ^ this.Key.GetHashCode();
                return hash;
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        #endregion
    }
}
