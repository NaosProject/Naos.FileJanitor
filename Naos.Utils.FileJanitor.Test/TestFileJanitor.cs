namespace Naos.Utils.FileJanitor.Test
{
    using Xunit;

    public class TestFileJanitor
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
