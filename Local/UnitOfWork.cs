using Local.Models;
using Shared.Interfaces;
using Shared.Interfaces.Repositories;

namespace Local;

public class UnitOfWork : IUnitOfWork<UserModel, CoinModel>
{
    private readonly object _lock = new();

    public UnitOfWork(IUserRepository<UserModel> userRepository, ICoinRepository<CoinModel> coinRepository)
    {
        UserRepository = userRepository;
        CoinRepository = coinRepository;
    }

    public IUserRepository<UserModel> UserRepository { get; init; }
    public ICoinRepository<CoinModel> CoinRepository { get; init; }

    public Task<IAtomicTransaction> BeginTransactionAsync()
    {
        return Task.FromResult<IAtomicTransaction>(new AtomicTransaction(_lock));
    }

    public Task<int> SaveChangesAsync()
    {
        //pass
        return Task.FromResult(-1);
    }
}