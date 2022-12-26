using System.Reflection;
using DbPostgres.Models;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces.Services;

namespace DbPostgresMigrator;

public static class Migrator
{
    public static void Migrate(string connectionString, IUsersProvider<UserModel> usersProvider)
    {
        var services = new ServiceCollection();
        services
            .AddTransient<IUsersProvider<UserModel>>(_ => usersProvider);
        services.AddMigrator(connectionString);
        var serviceProvider = services.BuildServiceProvider();
        var runner = serviceProvider.GetService<IMigrationRunner>();
        runner!.ListMigrations();
        runner.MigrateUp();
    }

    public static IServiceCollection AddMigrator(this IServiceCollection services, string connectionString)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.GetExecutingAssembly())
                .For.All());
        return services;
    }
}