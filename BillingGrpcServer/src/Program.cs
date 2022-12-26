using BillingGrpcServer.Extensions;
using BillingGrpcServer.GrpcServices;
using Shared.Services;
#if DBPOSTGRESQLBUILD
using DbPostgres.Models;
using DbPostgresMigrator;
using DbPostgresMigrator;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

#if DBPOSTGRESQLBUILD
var connectionString = builder.Configuration.GetSection("DatabaseConnectionOptions:ConnectionString").Get<string>();
builder.Services.AddDbBilling(connectionString);
Migrator.Migrate(connectionString, new FromDefaultJsonUsersProvider<UserModel>("defaultUserProfiles.json"));
#elif LOCALBUILD
builder.Services.AddLocalBilling();
#else
System.Environment.Exit(1);
#endif

var app = builder.Build();
app.MapGrpcService<BillingGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();