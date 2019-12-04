// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using FakeItEasy;
    using FluentAssertions;
    using Naos.FileJanitor.Domain;
    using Naos.FileJanitor.Serialization.Bson;
    using Naos.FileJanitor.Serialization.Json;
    using OBeautifulCode.Serialization.Bson;
    using OBeautifulCode.Serialization.Json;
    using Xunit;

    public static class SerializationTests
    {
        private static readonly ObcBsonSerializer BsonSerializer = new ObcBsonSerializer(typeof(FileJanitorBsonConfiguration));
        private static readonly ObcJsonSerializer JsonSerializer = new ObcJsonSerializer(typeof(FileJanitorJsonConfiguration));

        [Fact]
        public static void ArchivedDirectoryJanitor_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<ArchivedDirectory>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<ArchivedDirectory>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<ArchivedDirectory>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void FileLocationJanitor_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<FileLocation>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<FileLocation>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<FileLocation>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void MetadataItemJanitor_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<MetadataItem>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<MetadataItem>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<MetadataItem>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }
    }
}
