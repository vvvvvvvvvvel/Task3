using DbPostgres.DbContext;
using DbPostgres.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Models;
using Shared.Interfaces.Repositories;

namespace DbPostgres.Repositories;

public class CoinRepository : ICoinRepository<CoinModel>
{
    private readonly DbSet<CoinModel> _coins;

    public CoinRepository(BillingServiceContext db)
    {
        _coins = db.Coins;
    }

    public IQueryable<CoinModel> GetByOwner(string owner)
    {
        return _coins.Where(c => c.OwnerName == owner);
    }

    public async ValueTask CreateAsync(IUser user)
    {
        await _coins.AddAsync(new CoinModel(user));
    }

    public async ValueTask CreateAsync(IUser user, long amount)
    {
        await _coins.AddRangeAsync(new CoinModel[amount].Select(_ => new CoinModel(user)));
    }

    public async ValueTask<CoinModel> LongestHistoryCoinAsync()
    {
        CoinModel longestHistoryCoin;
        try
        {
            var max = await _coins.MaxAsync(c => c.HistoryLength);
            longestHistoryCoin = await _coins.FirstAsync(c => c.HistoryLength == max);
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("Empty coins collection");
        }

        return longestHistoryCoin;
    }
}