using Shared.Interfaces.Models;

namespace Shared.Interfaces.Services;

public interface IBillingService
{
    public Task<IEnumerable<IUser>> ListUsers();
    public Task<IResponse> CoinsEmission(long amount);
    public Task<IResponse> MoveCoins(string src, string dst, long amount);
    public Task<ICoin> LongestHistoryCoin();
}