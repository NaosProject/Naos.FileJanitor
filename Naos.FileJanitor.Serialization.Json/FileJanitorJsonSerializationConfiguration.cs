// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorJsonSerializationConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using Naos.FileJanitor;
    using Naos.FileJanitor.Domain;
    using OBeautifulCode.Serialization.Json;

    /// <summary>
    /// Implementation for the <see cref="FileJanitor" /> domain.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "FileJanitor", Justification = "Spelling/name is correct.")]
    public class FileJanitorJsonSerializationConfiguration : JsonSerializationConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<TypeToRegisterForJson> TypesToRegisterForJson => new[]
        {
            typeof(ArchivedDirectory).ToTypeToRegisterForJson(),
            typeof(FileLocation).ToTypeToRegisterForJson(),
            typeof(MetadataItem).ToTypeToRegisterForJson(),
        };
    }
}
