// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileArchivingTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Naos.FileJanitor.Core;
    using Naos.FileJanitor.Domain;

    using Xunit;

    public static class FileArchivingTest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "RoundTrip", Justification = "Spelling/name is correct.")]
        [Fact]
        public static async Task RoundTrip___ArchiveRestore___Directory()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                // Arrange
                var archiveFilePath = Path.Combine(tempPath, "Archive.zip");
                var directoryPathToArchive = Path.Combine(tempPath, "DirectoryToArchive");
                Directory.CreateDirectory(directoryPathToArchive);
                var directoryPathToRestore = Path.Combine(tempPath, "DirectoryThatWasRestoredInto");
                var filePath = Path.Combine(directoryPathToArchive, "Ninjas.txt");
                var content = "Ninjas Rule!";
                File.WriteAllText(filePath, content);
                var restoredFilePath = Path.Combine(Path.Combine(directoryPathToRestore, Path.GetDirectoryName(directoryPathToArchive) ?? string.Empty, filePath));

                var directoryArchiveKind = DirectoryArchiveKind.DotNetZipFile;
                var archiveCompressionKind = ArchiveCompressionKind.Smallest;
                var archiver = ArchiverFactory.Instance.BuildArchiver(directoryArchiveKind, archiveCompressionKind);

                // Act
                var result = await archiver.ArchiveDirectoryAsync(directoryPathToArchive, archiveFilePath);
                var restorer = ArchiverFactory.Instance.BuildArchiver(result);
                await restorer.RestoreDirectoryAsync(result, directoryPathToRestore);

                // Assert
                File.Exists(archiveFilePath).Should().BeTrue();
                result.DirectoryArchiveKind.Should().Be(directoryArchiveKind);
                result.ArchiveCompressionKind.Should().Be(archiveCompressionKind);
                File.Exists(restoredFilePath).Should().BeTrue();
                File.ReadAllText(restoredFilePath).Should().Be(content);
            }
            finally
            {
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
        }
    }
}
