using Billing.Models;

namespace Billing.Services;

public interface IEmissionStrategy
{
    public long[] CalcEmission(User[] users, long amount);
}