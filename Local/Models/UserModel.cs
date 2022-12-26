using Shared.Interfaces.Models;

namespace Local.Models;

public class UserModel : IUser
{
    public UserModel(string name, int rating, long amount)
    {
        Name = name;
        Rating = rating;
        Amount = amount;
    }

    public string Name { get; init; }
    public int Rating { get; set; }
    public long Amount { get; set; }
}