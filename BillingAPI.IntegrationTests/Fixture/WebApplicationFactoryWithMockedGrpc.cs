using BillingAPI.IntegrationTests.Mock;
using BillingAPI.IntegrationTests.Utils;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BillingAPI.IntegrationTests.Fixture;

public class WebApplicationFactoryWithMockedGrpc<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public enum ConfigType
    {
        Default,
        ZeroCoins,
        Unavailable
    }

    private void ChangeConfiguration(ConfigType configType, IWebHostBuilder builder)
    {
        var mockBillingClient = new Mock<Billing.BillingClient>();
        switch (configType)
        {
            case ConfigType.Default:
                mockBillingClient.Setup(m =>
                        m.LongestHistoryCoinAsync(It.IsAny<None>(), null, null, CancellationToken.None))
                    .Returns(CallHelpers.CreateAsyncUnaryCall(MockData.LongestHistoryCoin.GrpcContent));
                mockBillingClient.Setup(m =>
                        m.ListUsers(It.IsAny<None>(), null, null, CancellationToken.None))
                    .Returns(CallHelpers.CreateAsyncStreamingCall<UserProfile>(MockData.GetAll.GrpcContent));
                mockBillingClient.Setup(m =>
                        m.MoveCoinsAsync(It.Is<MoveCoinsTransaction>(t => t.Amount != 100), null, null,
                            CancellationToken.None))
                    .Returns(CallHelpers.CreateAsyncUnaryCall(MockData.MoveCoins.GrpcContent));
                mockBillingClient.Setup(m =>
                        m.MoveCoinsAsync(It.Is<MoveCoinsTransaction>(t => t.Amount == 100), null, null,
                            CancellationToken.None))
                    .Returns(CallHelpers.CreateAsyncUnaryCall(MockData.MoveCoins_CoinsLessThanNecessary.GrpcContent));
                mockBillingClient.Setup(m =>
                        m.CoinsEmissionAsync(It.Is<EmissionAmount>(t => t.Amount > 1), null, null,
                            CancellationToken.None))
                    .Returns(CallHelpers.CreateAsyncUnaryCall(MockData.CoinsEmission.GrpcContent));
                mockBillingClient.Setup(m =>
                        m.CoinsEmissionAsync(It.Is<EmissionAmount>(t => t.Amount == 1), null, null,
                            CancellationToken.None))
                    .Returns(CallHelpers.CreateAsyncUnaryCall(MockData.CoinsEmissionL_AmountLessThanUsers.GrpcContent));

                break;
            case ConfigType.ZeroCoins:
                mockBillingClient.Setup(
                        m =>
                            m.LongestHistoryCoinAsync(It.IsAny<None>(), null, null, CancellationToken.None))
                    .Throws(new RpcException(new Status(StatusCode.OutOfRange, "Empty coins collection")));
                break;
            case ConfigType.Unavailable:
                mockBillingClient.Setup(
                        m =>
                            m.LongestHistoryCoinAsync(It.IsAny<None>(), null, null, CancellationToken.None))
                    .Throws(new RpcException(new Status(StatusCode.Unavailable, "Error connecting to subchannel.")));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(configType), configType, null);
        }

        builder.ConfigureServices(services =>
        {
            var billingService = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(Billing.BillingClient));
            services.Remove(billingService!);
            services.AddSingleton(_ => mockBillingClient.Object);
        });
    }

    public HttpClient GetClient(
        ConfigType configType)
    {
        return WithWebHostBuilder(builder =>
                ChangeConfiguration(configType, builder))
            .CreateClient();
    }
}