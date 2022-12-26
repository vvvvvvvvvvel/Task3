using System.Net;
using BillingAPI.IntegrationTests.Fixture;
using BillingAPI.IntegrationTests.Mock;
using BillingAPI.IntegrationTests.Utils;
using FluentAssertions;

namespace BillingAPI.IntegrationTests;

public class GeneralTests : IClassFixture<WebApplicationFactoryWithMockedGrpc<BillingApiProgram>>
{
    private readonly WebApplicationFactoryWithMockedGrpc<BillingApiProgram> _factory;

    public GeneralTests(WebApplicationFactoryWithMockedGrpc<BillingApiProgram> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Grpc_Available()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Default);
        var actualInfo = await client.GetInfo(Method.Get, "/api/Coins/LongestHistoryCoin");
        actualInfo.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Grpc_Unavailable()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Unavailable);
        var actualInfo = await client.GetInfo(Method.Get, "/api/Coins/LongestHistoryCoin");
        actualInfo.Should()
            .BeEquivalentTo(MockData.Grpc_Unavailable.ExpectedResult);
    }
}