// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    using FluentAssertions;

    using Naos.FileJanitor.Domain;
    using Naos.FileJanitor.MessageBus.Scheduler;
    using Naos.MessageBus.Domain;
    using Naos.Serialization.Factory;

    using OBeautifulCode.TypeRepresentation;

    using Xunit;

    public class FileJanitorTest
    {
        [Fact]
        public void RoundtripHashAlgorithmName()
        {
            // Arrage
            var expected = new[] { HashAlgorithmName.MD5, HashAlgorithmName.SHA1, };
            var serializer = SerializerFactory.Instance.BuildSerializer(PostOffice.MessageSerializationDescription);

            // Act
            var actualString = serializer.SerializeToString(expected);
            var actualObject = serializer.Deserialize<IReadOnlyCollection<HashAlgorithmName>>(actualString);

            // Assert
            actualString.Should().NotBeNullOrWhiteSpace();
            actualObject.Should().NotBeNull();
            actualObject.Count.Should().Be(expected.Length);
            actualObject.First().Should().Be(expected.First());
            actualObject.Last().Should().Be(expected.Last());
        }

        [Fact]
        public void ShareFilePathMessageHandlerHandle_PathOnMessage_PathShared()
        {
            // arrange
            var message = new ShareFilePathMessage { FilePathToShare = "D:\\Monkey\\File.txt" };
            var handler = new ShareFilePathMessageHandler();

            // act
            handler.HandleAsync(message).Wait();

            // assert
            Assert.Equal(message.FilePathToShare, handler.FilePath);
        }

        [Fact]
        public void ShareFileLocationMessageHandlerHandle_PropertiesOnMessage_PropertiesShared()
        {
            // arrange
            var message = new ShareFileLocationMessage
                              {
                                  FileLocationToShare = new FileLocation { ContainerLocation = "region", Container = "bucket", Key = "key" },
                              };

            var handler = new ShareFileLocationMessageHandler();

            // act
            handler.HandleAsync(message).Wait();

            // assert
            Assert.Equal(message.FileLocationToShare.ContainerLocation, handler.FileLocation.ContainerLocation);
            Assert.Equal(message.FileLocationToShare.Container, handler.FileLocation.Container);
            Assert.Equal(message.FileLocationToShare.Key, handler.FileLocation.Key);
        }
    }
}
