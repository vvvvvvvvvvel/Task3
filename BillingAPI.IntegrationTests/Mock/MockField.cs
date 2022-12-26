namespace BillingAPI.IntegrationTests.Mock;

public class MockField
{
    public HttpContent? HttpContent { get; set; }
    public dynamic GrpcContent { get; set; }
    public Info ExpectedResult { get; init; }
}