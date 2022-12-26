using Shared.Interfaces.Models;

namespace Local.Models;

public class CoinModel : ICoin
{
    private static readonly object Locker = new();
    private static long _count;

    public readonly Stack<IUser> History;

    public CoinModel(IUser user)
    {
        lock (Locker)
        {
            Id = _count++;
        }

        History = new Stack<IUser>();
        History.Push(user);
    }

    public long Id { get; }

    public IUser Owner
    {
        get => History.Peek();
        set => History.Push(value);
    }

    public IEnumerable<string> GetHistory()
    {
        return History.Select(u => u.Name).Reverse().ToArray();
    }
}