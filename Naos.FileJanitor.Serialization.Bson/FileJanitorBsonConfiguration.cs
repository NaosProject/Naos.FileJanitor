// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorBsonConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Serialization.Bson
{
    using System;
    using System.Collections.Generic;
    using Naos.FileJanitor.Domain;
    using OBeautifulCode.Serialization.Bson;

    /// <summary>
    /// Implementation for the <see cref="FileJanitor" /> domain.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "FileJanitor", Justification = "Spelling/name is correct.")]
    public class FileJanitorBsonConfiguration : BsonConfigurationBase
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
