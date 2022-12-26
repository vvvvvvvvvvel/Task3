#if DBPOSTGRESQLBUILD
using BillingGrpcServer.Services;
using DbPostgres;
using DbPostgres.DbContext;
using DbPostgres.Models;
using DbPostgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;
using Shared.Interfaces.Repositories;
using Shared.Interfaces.Services;
using Shared.Services;

namespace BillingGrpcServer.Extensions;

public static class AddDbBillingServiceCollectionExtensions
{
    public static IServiceCollection AddDbBilling
        (this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BillingServiceContext>(options => options.UseNpgsql(connectionString));
        services.AddSingleton(typeof(IEmissionStrategy<>), typeof(DefaultEmissionStrategy<>));
        services.AddScoped<IUserRepository<UserModel>, UserRepository>();
        services.AddScoped<ICoinRepository<CoinModel>, CoinRepository>();
        services.AddScoped<IUnitOfWork<UserModel, CoinModel>, UnitOfWork>();
        services.AddScoped<IBillingService, BillingService<UserModel, CoinModel>>();
        return services;
    }
}
#endif