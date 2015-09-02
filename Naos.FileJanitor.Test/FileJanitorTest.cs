// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorTest.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.FileJanitor.MessageBus.Handler;

    using Xunit;

    public class FileJanitorTest
    {
        [Fact]
        public void GetTimeSpanFromDayHourMinuteColonDelimited_ValidData_ValidResult()
        {
            var raw = "00:04:00";
            var parsed = FileJanitorConsoleHarness.GetTimeSpanFromDayHourMinuteColonDelimited(raw);
            Assert.Equal(4, parsed.TotalHours);
        }

        [Fact]
        public void ShareFileMessageHandlerHandle_PathOnMessage_PathShared()
        {
            // arrange
            var message = new ShareFileMessage { FilePathToShare = "D:\\Monkey\\File.txt" };
            var handler = new ShareFileMessageHandler();

            // act
            handler.Handle(message);

            // assert
            Assert.Equal(message.FilePathToShare, handler.FilePath);
        }
    }
}
