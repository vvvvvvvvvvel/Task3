using Shared.Interfaces.Models;

namespace Shared.Interfaces.Services;

public interface IEmissionStrategy<in TUser> where TUser : IUser
{
    public long[] CalcEmission(TUser[] users, long amount);
}