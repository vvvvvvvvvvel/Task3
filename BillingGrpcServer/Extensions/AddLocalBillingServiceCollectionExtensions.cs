#if LOCALBUILD
using BillingGrpcServer.Services;
using Local;
using Local.Models;
using Local.Repositories;
using Shared.Interfaces;
using Shared.Interfaces.Repositories;
using Shared.Interfaces.Services;
using Shared.Services;

namespace BillingGrpcServer.Extensions;

public static class AddLocalBillingServiceCollectionExtensions
{
    public static IServiceCollection AddLocalBilling
        (this IServiceCollection services)
    {
        services.AddTransient<IUsersProvider<UserModel>>(serviceProvider =>new FromDefaultJsonUsersProvider<UserModel>("defaultUserProfiles.json"));
        services.AddSingleton<IEmissionStrategy<UserModel>, DefaultEmissionStrategy<UserModel>>();
        services.AddSingleton<IUserRepository<UserModel>, UserRepository>();
        services.AddSingleton<ICoinRepository<CoinModel>, CoinRepository>();
        services.AddSingleton<IUnitOfWork<UserModel, CoinModel> , UnitOfWork>();
        services.AddSingleton<IBillingService, BillingService<UserModel, CoinModel>>();
        return services;
    }
}
#endif