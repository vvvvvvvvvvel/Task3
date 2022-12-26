using Grpc.Net.Client;

namespace BillingAPI;

public static class AddBillingGprsClientCollectionExtension
{
    private static GrpcChannel _grpcChannel;

    public static IServiceCollection AddBillingClient
        (this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<IConfiguration>();
        var backendUrl = config["BackendUrl"];
        if (backendUrl is null) throw new InvalidOperationException("empty config[\"BackendUrl\"]");

        _grpcChannel = GrpcChannel.ForAddress(backendUrl);
        services.AddTransient(_ => new Billing.BillingClient(_grpcChannel));
        return services;
    }
}