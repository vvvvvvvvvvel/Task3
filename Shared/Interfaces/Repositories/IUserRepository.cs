using System.Linq.Expressions;
using Shared.Interfaces.Models;

namespace Shared.Interfaces.Repositories;

public interface IUserRepository<TUser>
    where TUser : IUser
{
    public IQueryable<TUser> GetAll();
    public ValueTask<TUser?> FirstOrDefaultAsync(Expression<Func<TUser, bool>> predicate);
}