using System.Linq.Expressions;
using DbPostgres.DbContext;
using DbPostgres.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Repositories;

namespace DbPostgres.Repositories;

public class UserRepository : IUserRepository<UserModel>
{
    private readonly DbSet<UserModel> _users;

    public UserRepository(BillingServiceContext db)
    {
        _users = db.Users;
    }

    public IQueryable<UserModel> GetAll()
    {
        return _users;
    }

    public async ValueTask<UserModel?> FirstOrDefaultAsync(Expression<Func<UserModel, bool>> predicate)
    {
        return await _users.FirstOrDefaultAsync(predicate);
    }
}