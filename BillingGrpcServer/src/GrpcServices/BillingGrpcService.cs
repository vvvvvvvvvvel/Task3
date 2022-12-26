using Grpc.Core;
using Shared.Interfaces.Models;
using Shared.Interfaces.Services;

namespace BillingGrpcServer.GrpcServices;

public class BillingGrpcService : Billing.BillingBase
{
    private readonly IBillingService _billingService;
    private readonly ILogger<BillingGrpcService> _logger;


    public BillingGrpcService(IBillingService billingService, ILogger<BillingGrpcService> logger)
    {
        _billingService = billingService;
        _logger = logger;
    }

    public override async Task ListUsers(None request, IServerStreamWriter<UserProfile> responseStream,
        ServerCallContext context)
    {
        foreach (var user in (await _billingService.ListUsers()).TakeWhile(user =>
                     !context.CancellationToken.IsCancellationRequested))
            await responseStream.WriteAsync(new UserProfile
            {
                Name = user.Name,
                Amount = user.Amount
            });
    }

    private static Response CompileGrpcResponse(IResponse response)
    {
        var (responseStatus, comment) = response;
        return new Response
        {
            Status = responseStatus switch
            {
                IResponse.Status.Ok => Response.Types.Status.Ok,
                IResponse.Status.Failed => Response.Types.Status.Failed,
                IResponse.Status.Unspecified => Response.Types.Status.Unspecified,
                _ => throw new ArgumentOutOfRangeException()
            },
            Comment = comment
        };
    }

    public override async Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
    {
        var result = await _billingService.CoinsEmission(request.Amount);
        return CompileGrpcResponse(result);
    }

    public override async Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
    {
        var result = await _billingService.MoveCoins(request.SrcUser, request.DstUser, request.Amount);
        return CompileGrpcResponse(result);
    }

    public override async Task<Coin>
        LongestHistoryCoin(None request,
            ServerCallContext context) //не предусмотрено, что у 2х монет будет одинаково длинная история
    {                                  //т.к. не понятно из задачи, нужно ли это
        try
        {
            var coin = await _billingService.LongestHistoryCoin();
            return new Coin
            {
                Id = coin.Id,
                History = coin.GetHistory().Aggregate((prev, current) => $"{prev} → {current}")
            };
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.OutOfRange, ex.Message), ex.Message);
        }
    }
}