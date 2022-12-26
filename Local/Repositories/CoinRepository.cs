using Local.Models;
using Shared.Interfaces.Models;
using Shared.Interfaces.Repositories;

namespace Local.Repositories;

public class CoinRepository : ICoinRepository<CoinModel>
{
    private readonly HashSet<CoinModel> _coins;

    public CoinRepository()
    {
        _coins = new HashSet<CoinModel>();
    }

    public ValueTask CreateAsync(IUser user)
    {
        _coins.Add(new CoinModel(user));
        return ValueTask.CompletedTask;
    }

    public ValueTask CreateAsync(IUser user, long amount)
    {
        var coins = new CoinModel[amount].Select(_ => new CoinModel(user));
        _coins.UnionWith(coins);
        return ValueTask.CompletedTask;
    }

    public IQueryable<CoinModel> GetByOwner(string owner)
    {
        return _coins.Where(c => c.Owner.Name == owner).AsQueryable();
    }


    public ValueTask<CoinModel> LongestHistoryCoinAsync()
    {
        return ValueTask.FromResult(_coins.MaxBy(c => c.History.Count)!);
    }
}