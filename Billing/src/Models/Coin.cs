namespace Billing.Models;

public class Coin
{
    public long Id { get; init; }
    public readonly Stack<User> History;

    public Coin(long id, User user)
    {
        Id = id;
        History = new Stack<User>();
        History.Push(user);
    }
}