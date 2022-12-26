using System.Net;
using System.Text;

namespace BillingAPI.IntegrationTests.Mock;

public static class MockData
{
    public static readonly MockField LongestHistoryCoin = new()
    {
        GrpcContent = new Coin
        {
            Id = 1,
            History = "oleg"
        },
        ExpectedResult = new Info(@"{""id"":1,""history"":""oleg""}", HttpStatusCode.OK,
            "application/json; charset=utf-8")
    };

    public static readonly MockField GetAll = new()
    {
        GrpcContent = new List<UserProfile>
        {
            new()
            {
                Name = "oleg",
                Amount = 2
            },
            new()
            {
                Name = "maria",
                Amount = 5
            }
        },
        ExpectedResult = new Info(
            @"[{""name"":""oleg"",""amount"":2},{""name"":""maria"",""amount"":5}]",
            HttpStatusCode.OK, "application/json; charset=utf-8"
        )
    };

    public static readonly MockField MoveCoins = new()
    {
        HttpContent = new StringContent(@"{""srcUser"":""maria"",""dstUser"":""oleg"",""amount"":1}",
            Encoding.UTF8, "application/json"),
        GrpcContent = new Response
        {
            Status = Response.Types.Status.Ok,
            Comment = "Done: maria 1,oleg 2"
        },
        ExpectedResult = new Info(
            @"{""responseStatus"":""Ok"",""comment"":""Done: maria 1,oleg 2""}",
            HttpStatusCode.OK, "application/json; charset=utf-8")
    };

    public static readonly MockField CoinsEmission = new()
    {
        HttpContent = new StringContent(@"{""amount"":10}",
            Encoding.UTF8, "application/json"),
        GrpcContent = new Response
        {
            Status = Response.Types.Status.Ok,
            Comment = "Done"
        },
        ExpectedResult = new Info(@"{""responseStatus"":""Ok"",""comment"":""Done""}", HttpStatusCode.OK,
            "application/json; charset=utf-8")
    };

    public static readonly MockField CoinsEmissionL_AmountLessThanUsers = new()
    {
        HttpContent = new StringContent(@"{""amount"":1}",
            Encoding.UTF8, "application/json"),
        GrpcContent = new Response
        {
            Status = Response.Types.Status.Failed,
            Comment = "Amount less than number of users"
        },
        ExpectedResult = new Info(
            @"{""error"":""Operation failed"",""description"":""Amount less than number of users""}",
            HttpStatusCode.BadRequest,
            "application/json; charset=utf-8")
    };

    public static readonly MockField LongestHistoryCoin_EmptyCoinsCollection = new()
    {
        ExpectedResult = new Info(@"{""error"":""Out Of Range"",""description"":""Empty coins collection""}",
            HttpStatusCode.BadRequest,
            "application/json; charset=utf-8")
    };

    public static readonly MockField Grpc_Unavailable = new()
    {
        ExpectedResult = new Info(
            @"{""error"":""Internal Server Error"",""description"":""Error connecting to subchannel.""}",
            HttpStatusCode.InternalServerError,
            "application/json; charset=utf-8")
    };

    public static readonly MockField MoveCoins_CoinsLessThanNecessary = new()
    {
        HttpContent = new StringContent(@"{""srcUser"":""maria"",""dstUser"":""oleg"",""amount"":100}",
            Encoding.UTF8, "application/json"),
        GrpcContent = new Response
        {
            Status = Response.Types.Status.Failed,
            Comment = "Insufficient coins"
        },
        ExpectedResult = new Info(@"{""error"":""Operation failed"",""description"":""Insufficient coins""}",
            HttpStatusCode.BadRequest,
            "application/json; charset=utf-8")
    };
}