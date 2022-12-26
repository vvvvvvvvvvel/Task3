namespace BillingAPI.IntegrationTests.Utils;

public static class HttpTestHelper
{
    public static async Task<Info> GetInfo(this HttpClient client, Method httpMethod, string url,
        HttpContent? content = null)
    {
        var method = httpMethod switch
        {
            Method.Get => HttpMethod.Get,
            Method.Post => HttpMethod.Post,
            _ => throw new ArgumentException()
        };

        var httpMessage = new HttpRequestMessage(method, url) {Content = content};
        var response = await client.SendAsync(httpMessage);
        var respContent = await response.Content.ReadAsStringAsync();
        return new Info(respContent, response.StatusCode, response.Content.Headers.ContentType!.ToString());
    }
}