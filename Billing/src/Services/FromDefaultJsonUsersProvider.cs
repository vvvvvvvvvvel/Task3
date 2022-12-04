using System.Data;
using System.Text;
using System.Text.Json;
using Billing.Models;


namespace Billing.Services;

public class FromDefaultJsonUsersProvider : IUsersProvider
{
    public List<User> Get()
    {
        try
        {
            var jsonUserProfiles = File.ReadAllText("defaultUserProfiles.json", Encoding.Default);
            var tmpUsers = JsonSerializer.Deserialize<List<User>>(jsonUserProfiles)!;
            if (tmpUsers.Count != tmpUsers.DistinctBy(u => u.Name).Count())
            {
                throw new DuplicateNameException(
                    "Not possible to add users with same Name, because MoveCoin is performed by Name");
            }

            return tmpUsers;
        }
        catch (Exception)
        {
            throw new Exception($"Can`t load defaultUserProfiles.json");
        }
    }
}