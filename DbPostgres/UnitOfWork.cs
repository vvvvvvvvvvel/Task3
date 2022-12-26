using DbPostgres.DbContext;
using DbPostgres.Models;
using Shared.Interfaces;
using Shared.Interfaces.Repositories;

namespace DbPostgres;

public class UnitOfWork : IUnitOfWork<UserModel, CoinModel>
{
    private readonly BillingServiceContext _db;

    public UnitOfWork(BillingServiceContext db, IUserRepository<UserModel> userRepository,
        ICoinRepository<CoinModel> coinRepository)
    {
        _db = db;
        UserRepository = userRepository;
        CoinRepository = coinRepository;
    }

    public IUserRepository<UserModel> UserRepository { get; init; }
    public ICoinRepository<CoinModel> CoinRepository { get; init; }

    public async Task<IAtomicTransaction> BeginTransactionAsync()
    {
        var transaction = await AtomicTransaction.CreateAsync(_db.Database);
        return transaction;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }
}