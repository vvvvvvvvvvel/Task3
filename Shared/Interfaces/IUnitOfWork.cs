using Shared.Interfaces.Models;
using Shared.Interfaces.Repositories;

namespace Shared.Interfaces;

public interface IUnitOfWork<TUser, TCoin>
    where TUser : IUser
    where TCoin : ICoin
// where TAtomicTransaction: IAtomicTransaction
{
    public IUserRepository<TUser> UserRepository { get; init; }
    public ICoinRepository<TCoin> CoinRepository { get; init; }
    public Task<IAtomicTransaction> BeginTransactionAsync();
    public Task<int> SaveChangesAsync();
}