using Jot.Core.Interfaces;

namespace Jot.Tests
{
    public class TestClock : IClock
    {
        public TestClock(Func<DateTime> dateTime)
        {
            DateTime = dateTime;
        }

        public Func<DateTime> DateTime { get; }

        public DateTimeOffset Now => DateTime();
    }
}
