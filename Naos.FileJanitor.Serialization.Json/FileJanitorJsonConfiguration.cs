// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorJsonConfiguration.cs" company="Naos Project">
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
    public class FileJanitorJsonConfiguration : JsonConfigurationBase
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> TypesToAutoRegister => new[]
        {
            typeof(ArchivedDirectory),
            typeof(FileLocation),
            typeof(MetadataItem),
        };
    }
}
