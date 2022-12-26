using System.ComponentModel.DataAnnotations.Schema;
using Shared.Interfaces.Models;

namespace DbPostgres.Models;

public class CoinModel : ICoin
{
    public readonly string[] History;
    public readonly int HistoryLength;

    public string OwnerName;

    public CoinModel(long id, string ownerName, string[] history, int historyLength)
    {
        Id = id;
        OwnerName = ownerName;
        History = history;
        HistoryLength = historyLength;
    }

    public CoinModel(IUser owner)
    {
        OwnerName = owner.Name;
    }

    public virtual UserModel OwnerNavigation { get; set; } = null!;
    public long Id { get; set; }

    public IEnumerable<string> GetHistory()
    {
        return History;
    }

    [NotMapped]
    public IUser Owner
    {
        get => OwnerNavigation;
        set => OwnerNavigation = (UserModel) value;
    }
}