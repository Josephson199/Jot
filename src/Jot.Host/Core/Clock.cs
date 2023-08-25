using Jot.Core.Interfaces;

namespace Jot.Core
{
    public static class Clock
    {
        public static IClock Default { get; private set; } = new SystemClock();

        public static void Set(IClock clock)
        {
            Default = clock;
        }

        private class SystemClock : IClock
        {
            public DateTimeOffset Now => DateTime.UtcNow;
        }
    }
}
