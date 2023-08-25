namespace Jot.Core.Interfaces
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
    }
}
