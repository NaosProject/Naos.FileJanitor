// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryArchiveKind.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Domain
{
    /// <summary>
    /// Enumeration of kinds of directory archival.
    /// </summary>
    public enum DirectoryArchiveKind
    {
        /// <summary>
        /// Invalid default option.
        /// </summary>
        Invalid,

        /// <summary>
        /// Uses .NET ZipFile class.
        /// </summary>
        DotNetZipFile,
    }

    /// <summary>
    /// Enumeraion of compression options when archiving.
    /// </summary>
    public enum ArchiveCompressionKind
    {
        /// <summary>
        /// Invalid default option.
        /// </summary>
        Invalid,

        /// <summary>
        /// No compression.
        /// </summary>
        None,

        /// <summary>
        /// Smallest output compression, sacrifice time.
        /// </summary>
        Smallest,

        /// <summary>
        /// Fastest compression, sacrifice size.
        /// </summary>
        Fastest,
    }
}
