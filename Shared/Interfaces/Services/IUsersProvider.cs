using Shared.Interfaces.Models;

namespace Shared.Interfaces.Services;

public interface IUsersProvider<TUser> where TUser : IUser
{
    public List<TUser> Get();
}