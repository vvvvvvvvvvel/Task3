using Shared.Interfaces.Models;

namespace DbPostgres.Models;

public class UserModel : IUser
{
    public UserModel(string name, int rating, long amount)
    {
        Name = name;
        Rating = rating;
        Amount = amount;
    }

    public virtual ICollection<CoinModel> Coins { get; } = new List<CoinModel>();
    public string Name { get; init; }
    public int Rating { get; set; }
    public long Amount { get; set; }
}