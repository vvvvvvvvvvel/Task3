using Billing.Models;
using Billing.Services;
using Grpc.Core;

namespace Billing.GrpcServices;

public class BillingGrpcService: Billing.BillingBase
{
    private readonly ILogger<BillingGrpcService> _logger;
    private readonly IBillingService _billingService;

    public BillingGrpcService(ILogger<BillingGrpcService> logger, IBillingService billingService)
    {
        _logger = logger;
        this._billingService = billingService;
    }

    public override async Task ListUsers(None request, IServerStreamWriter<UserProfile> responseStream, ServerCallContext context)
    {
        foreach (var user in _billingService.ListUsers().TakeWhile(user => !context.CancellationToken.IsCancellationRequested))
        {
            await responseStream.WriteAsync(new UserProfile
            {
                Name = user.Name,
                Amount = user.Amount
            });
        }
    }

    private static Response CompileGrpcResponse(ResponseViewModel response)
    {
        var (responseStatus, comment) = response;
        return new Response
        {
            Status = responseStatus switch
            {
                ResponseViewModel.Status.Ok => Response.Types.Status.Ok,
                ResponseViewModel.Status.Failed => Response.Types.Status.Failed,
                ResponseViewModel.Status.Unspecified => Response.Types.Status.Unspecified,
                _ => throw new ArgumentOutOfRangeException()
            },
            Comment = comment
        };
    }

    public override Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
    {
        var result = _billingService.CoinsEmission(request.Amount);
        return Task.FromResult(CompileGrpcResponse(result));
    }

    public override Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
    {
        var result = _billingService.MoveCoins(request.SrcUser, request.DstUser, request.Amount);
        return Task.FromResult(CompileGrpcResponse(result));
    }

    public override Task<Coin> LongestHistoryCoin(None request, ServerCallContext context) //не предусмотрено, что у 2х монет будет одинаково длинная история
    {                                                                                      //т.к. не понятно из задачи, нужно ли это
        try
        {
            var coin = _billingService.LongestHistoryCoin();
            return Task.FromResult(new Coin
            {
                Id = coin.Id,
                History = coin.History.Select(u=>u.Name).Aggregate((current, prev) => $"{prev} → {current}")
            });
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.OutOfRange, ex.Message), ex.Message);
        }
    }
}