using Billing.Models;

namespace Billing.Services;

public interface IUsersProvider
{
    public List<User> Get();
}