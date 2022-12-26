using System.Data;
using System.Text;
using System.Text.Json;
using Shared.Interfaces.Models;
using Shared.Interfaces.Services;

namespace Shared.Services;

public class FromDefaultJsonUsersProvider<TUsers> : IUsersProvider<TUsers> where TUsers : IUser
{
    private readonly string _path;
    private List<TUsers> _users;

    public FromDefaultJsonUsersProvider(string path)
    {
        _path = path;
    }

    public List<TUsers> Get()
    {
        if (_users is not null)
            return _users;
        try
        {
            var jsonUserProfiles = File.ReadAllText(_path, Encoding.Default);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var tmpUsers = JsonSerializer.Deserialize<List<TUsers>>(jsonUserProfiles, serializeOptions)!;
            if (tmpUsers.Count != tmpUsers.DistinctBy(u => u.Name).Count())
                throw new DuplicateNameException(
                    "Not possible to add users with same Name, because MoveCoin is performed by Name");

            _users = tmpUsers;
            return _users;
        }
        catch (Exception)
        {
            throw new Exception("Can`t load defaultUserProfiles.json");
        }
    }
}