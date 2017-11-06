// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using Naos.FileJanitor.Console;
    using Naos.FileJanitor.Domain;
    using Naos.FileJanitor.MessageBus.Scheduler;

    using Xunit;

    public class FileJanitorTest
    {
        [Fact]
        public void GetTimeSpanFromDayHourMinuteColonDelimited_ValidData_ValidResult()
        {
            var raw = "00:04:00";
            var parsed = ConsoleAbstraction.GetTimeSpanFromDayHourMinuteColonDelimited(raw);
            Assert.Equal(4, parsed.TotalHours);
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
