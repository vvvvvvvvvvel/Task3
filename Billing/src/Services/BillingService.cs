using Billing.Exceptions;
using Billing.Models;


namespace Billing.Services;

public class BillingService : IBillingService
{
    private readonly HashSet<Models.Coin> _coins;
    private readonly List<Models.User> _users;
    private readonly ReaderWriterLock _coinLock = new();
    private readonly IEmissionStrategy _emissionStrategy;

    public BillingService(IEmissionStrategy emissionStrategy, IUsersProvider usersProvider)
    {
        _emissionStrategy = emissionStrategy;
        _coins = new HashSet<Models.Coin>();
        _users = usersProvider.Get();
    }

    public IEnumerable<User> ListUsers()
    {
        return _users;
    }

    public ResponseViewModel CoinsEmission(long amount)
    {
        try
        {
            var rewards = _emissionStrategy.CalcEmission(_users.ToArray(), amount);
            try
            {
                _coinLock.AcquireWriterLock(100);
                for (var i = 0; i < _users.Count; i++)
                {
                    AddCoins(_users[i], rewards[i]);
                }
            }
            finally
            {
                _coinLock.ReleaseWriterLock();
            }

            return new ResponseViewModel(ResponseViewModel.Status.Ok, "Done");
        }
        catch (AmountLessThanNumbersOfUsersException ex)
        {
            return new ResponseViewModel(ResponseViewModel.Status.Failed, ex.Message);
        }
    }

    private void AddCoins(User user, long amount)
    {
        for (var j = 0; j < amount; j++)
        {
            _coins.Add(new Models.Coin(_coins.Count, user));
        }

        user.Amount += amount;
    }

    private ResponseViewModel MoveCoins(User srcUser, User dstUser, long amount)
    {
        if (srcUser == dstUser)
        {
            return new ResponseViewModel(ResponseViewModel.Status.Failed,
                $"Argument error: you can't transfer coins to yourself");
        }

        if (srcUser.Amount < amount)
        {
            return new ResponseViewModel(ResponseViewModel.Status.Failed,
                $"Insufficient coins {srcUser.Amount} < {amount}");
        }

        try
        {
            _coinLock.AcquireWriterLock(100);
            var availableCoins = _coins.Where(c => c.History.Peek() == srcUser).Take((int)amount).ToArray();
            foreach (var coin in availableCoins)
            {
                coin.History.Push(dstUser);
            }

            srcUser.Amount -= amount;
            dstUser.Amount += amount;
        }
        finally
        {
            _coinLock.ReleaseWriterLock();
        }

        return new ResponseViewModel(ResponseViewModel.Status.Ok,
            $"Done: {srcUser.Name} {srcUser.Amount}, {dstUser.Name} {dstUser.Amount}");
    }

    public ResponseViewModel MoveCoins(string src, string dst, long amount)
    {
        var srcUser = _users.FirstOrDefault(u => u.Name == src);
        var dstUser = _users.FirstOrDefault(u => u.Name == dst);
        if (srcUser is not null && dstUser is not null) return MoveCoins(srcUser, dstUser, amount);
        var nonExistUser = srcUser is null ? src : dst;
        return new ResponseViewModel(ResponseViewModel.Status.Failed,
            $"User {nonExistUser} not found");
    }

    public Models.Coin LongestHistoryCoin()
    {
        Models.Coin longestHistoryCoin;
        try
        {
            _coinLock.AcquireReaderLock(100);
            longestHistoryCoin = _coins.MaxBy(c => c.History.Count)!;
        }
        finally
        {
            _coinLock.ReleaseReaderLock();
        }

        if (longestHistoryCoin is null)
        {
            throw new InvalidOperationException("Empty coins collection");
        }

        return longestHistoryCoin;
    }
}