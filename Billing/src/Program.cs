using Billing;
using Billing.GrpcServices;
using Billing.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddBilling<BillingService, DefaultEmissionStrategy, FromDefaultJsonUsersProvider>();

var app = builder.Build();

app.MapGrpcService<BillingGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();