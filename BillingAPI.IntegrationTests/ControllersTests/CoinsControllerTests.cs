using BillingAPI.IntegrationTests.Fixture;
using BillingAPI.IntegrationTests.Mock;
using BillingAPI.IntegrationTests.Utils;
using FluentAssertions;

namespace BillingAPI.IntegrationTests.ControllersTests;

public class CoinsControllerTests : IClassFixture<WebApplicationFactoryWithMockedGrpc<BillingApiProgram>>
{
    private readonly WebApplicationFactoryWithMockedGrpc<BillingApiProgram> _factory;

    public CoinsControllerTests(WebApplicationFactoryWithMockedGrpc<BillingApiProgram> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_LongestHistoryCoin()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Default);
        var actualInfo = await client.GetInfo(Method.Get, "/api/Coins/LongestHistoryCoin");
        actualInfo.Should()
            .BeEquivalentTo(MockData.LongestHistoryCoin.ExpectedResult);
    }


    [Fact]
    public async Task Get_LongestHistoryCoin_ZeroCoins()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.ZeroCoins);
        var actualInfo = await client.GetInfo(Method.Get, "/api/Coins/LongestHistoryCoin");
        actualInfo.Should()
            .BeEquivalentTo(MockData.LongestHistoryCoin_EmptyCoinsCollection.ExpectedResult);
    }

    [Fact]
    public async Task Post_MoveCoins()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Default);
        var actualInfo = await client.GetInfo(Method.Post, "/api/Coins/Move", MockData.MoveCoins.HttpContent);
        actualInfo.Should()
            .BeEquivalentTo(MockData.MoveCoins.ExpectedResult);
    }

    [Fact]
    public async Task Post_MoveCoins_ErrorProcessing()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Default);
        var actualInfo = await client.GetInfo(Method.Post, "/api/Coins/Move",
            MockData.MoveCoins_CoinsLessThanNecessary.HttpContent);
        actualInfo.Should()
            .BeEquivalentTo(MockData.MoveCoins_CoinsLessThanNecessary.ExpectedResult);
    }

    [Fact]
    public async Task Post_CoinsEmission()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Default);
        var actualInfo = await client.GetInfo(Method.Post, "/api/Coins/Emission", MockData.CoinsEmission.HttpContent);
        actualInfo.Should()
            .BeEquivalentTo(MockData.CoinsEmission.ExpectedResult);
    }

    [Fact]
    public async Task Post_CoinsEmission_ErrorProcessing()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Default);
        var actualInfo = await client.GetInfo(Method.Post, "/api/Coins/Emission",
            MockData.CoinsEmissionL_AmountLessThanUsers.HttpContent);
        actualInfo.Should()
            .BeEquivalentTo(MockData.CoinsEmissionL_AmountLessThanUsers.ExpectedResult);
    }
}