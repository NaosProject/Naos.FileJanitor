// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorDummyFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Recipes
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using FakeItEasy;
    using Naos.FileJanitor;
    using Naos.FileJanitor.Domain;
    using OBeautifulCode.AutoFakeItEasy;

    /// <summary>
    /// A dummy factory for Accounting Time types.
    /// </summary>
#if !NaosFileJanitorRecipesProject
    [System.Diagnostics.DebuggerStepThrough]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Naos.FileJanitor", "See package version number")]
#endif
    public class FileJanitorDummyFactory : IDummyFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileJanitorDummyFactory"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is not excessively complex.  Dummy factories typically wire-up many types.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is not excessively complex.  Dummy factories typically wire-up many types.")]
        public FileJanitorDummyFactory()
        {
            AutoFixtureBackedDummyFactory.ConstrainDummyToExclude(DirectoryArchiveKind.Invalid);
            AutoFixtureBackedDummyFactory.ConstrainDummyToExclude(ArchiveCompressionKind.Invalid);

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var result = new ArchivedDirectory(A.Dummy<DirectoryArchiveKind>(), A.Dummy<ArchiveCompressionKind>(), A.Dummy<string>(), A.Dummy<bool>(), A.Dummy<Encoding>().WebName, A.Dummy<DateTime>().ToUniversalTime());

                    return result;
                });
        }

        /// <inheritdoc />
        public Priority Priority => new FakeItEasy.Priority(1);

        /// <inheritdoc />
        public bool CanCreate(Type type)
        {
            return false;
        }

        /// <inheritdoc />
        public object Create(Type type)
        {
            return null;
        }
    }
}
