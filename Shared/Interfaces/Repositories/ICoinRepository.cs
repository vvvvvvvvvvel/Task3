using Shared.Interfaces.Models;

namespace Shared.Interfaces.Repositories;

public interface ICoinRepository<TCoin>
    where TCoin : ICoin
{
    public ValueTask CreateAsync(IUser user);
    public ValueTask CreateAsync(IUser user, long amount);
    public ValueTask<TCoin> LongestHistoryCoinAsync();
    public IQueryable<TCoin> GetByOwner(string ownerName);
}