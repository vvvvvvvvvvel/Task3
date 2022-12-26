using System.Linq.Expressions;
using Local.Models;
using Shared.Interfaces.Repositories;
using Shared.Interfaces.Services;

namespace Local.Repositories;

public class UserRepository : IUserRepository<UserModel>
{
    private readonly List<UserModel> _users;

    public UserRepository(IUsersProvider<UserModel> usersProvider)
    {
        _users = usersProvider.Get();
    }

    public IQueryable<UserModel> GetAll()
    {
        return _users.AsQueryable();
    }

    public ValueTask<UserModel?> FirstOrDefaultAsync(Expression<Func<UserModel, bool>> predicate)
    {
        return ValueTask.FromResult(_users.FirstOrDefault(predicate.Compile()));
    }
}