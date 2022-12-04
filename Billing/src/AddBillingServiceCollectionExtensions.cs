using Billing.Services;

namespace Billing;

public static class AddBillingServiceCollectionExtensions
{
    public static IServiceCollection AddBilling<TBillingService, TEmissionStrategy, TUsersProvider>
        (this IServiceCollection services)
        where TBillingService : class, IBillingService
        where TEmissionStrategy : class, IEmissionStrategy
        where TUsersProvider : class, IUsersProvider
    {
        services.AddSingleton<IUsersProvider, TUsersProvider>();
        services.AddSingleton<IEmissionStrategy, TEmissionStrategy>();
        services.AddSingleton<IBillingService, TBillingService>();
        return services;
    }
}