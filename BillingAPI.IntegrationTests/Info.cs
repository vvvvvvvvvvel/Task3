using System.Net;

namespace BillingAPI.IntegrationTests;

public record Info(string Content, HttpStatusCode StatusCode, string ContentType);