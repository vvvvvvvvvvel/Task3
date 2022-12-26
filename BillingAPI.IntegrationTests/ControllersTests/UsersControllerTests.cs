using BillingAPI.IntegrationTests.Fixture;
using BillingAPI.IntegrationTests.Mock;
using BillingAPI.IntegrationTests.Utils;
using FluentAssertions;

namespace BillingAPI.IntegrationTests.ControllersTests;

public class UsersControllerTests : IClassFixture<WebApplicationFactoryWithMockedGrpc<BillingApiProgram>>
{
    private readonly WebApplicationFactoryWithMockedGrpc<BillingApiProgram> _factory;

    public UsersControllerTests(WebApplicationFactoryWithMockedGrpc<BillingApiProgram> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_GetAll()
    {
        var client = _factory.GetClient(WebApplicationFactoryWithMockedGrpc<BillingApiProgram>.ConfigType.Default);
        var actualInfo = await client.GetInfo(Method.Get, "/api/Users/GetAll");
        actualInfo.Should()
            .BeEquivalentTo(MockData.GetAll.ExpectedResult);
    }
}