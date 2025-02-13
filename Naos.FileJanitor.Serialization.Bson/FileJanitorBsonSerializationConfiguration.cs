// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorBsonSerializationConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Serialization.Bson
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Naos.FileJanitor.Domain;
    using OBeautifulCode.Serialization.Bson;

    /// <summary>
    /// Implementation for the <see cref="FileJanitor" /> domain.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "FileJanitor", Justification = "Spelling/name is correct.")]
    public class FileJanitorBsonSerializationConfiguration : BsonSerializationConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<string> TypeToRegisterNamespacePrefixFilters => new[]
        {
            typeof(ArchivedDirectory).Namespace,
        };

        /// <inheritdoc />
        protected override IReadOnlyCollection<TypeToRegisterForBson> TypesToRegisterForBson => new[]
        {
            typeof(ArchivedDirectory).ToTypeToRegisterForBson(),
            typeof(FileLocation).ToTypeToRegisterForBson(),
            typeof(MetadataItem).ToTypeToRegisterForBson(),
            typeof(MultipleKeysFoundStrategy).ToTypeToRegisterForBson(),
        };
    }
}
