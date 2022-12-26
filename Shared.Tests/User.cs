using Shared.Interfaces.Models;

namespace Shared.Tests;

public class User : IUser
{
    public User(string name, int rating, long amount)
    {
        Name = name;
        Rating = rating;
        Amount = amount;
    }

    public string Name { get; init; }
    public int Rating { get; set; }

    public long Amount { get; set; }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not User otherUser)
            return false;

        if (ReferenceEquals(this, otherUser))
            return true;

        return Name == otherUser.Name &&
               Rating == otherUser.Rating && Amount == otherUser.Amount;
    }
}