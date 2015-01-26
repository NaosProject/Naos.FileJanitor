// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorTest.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
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
    }
}
