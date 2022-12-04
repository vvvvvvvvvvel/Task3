using Billing.Models;


namespace Billing.Services;

public interface IBillingService
{
    public IEnumerable<User> ListUsers();
    public ResponseViewModel CoinsEmission(long amount);
    public ResponseViewModel MoveCoins(string src, string dst, long amount);
    public Models.Coin LongestHistoryCoin();
}