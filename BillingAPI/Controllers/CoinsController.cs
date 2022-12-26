using AutoMapper;
using BillingAPI.Models;
using BillingAPI.Validation;
using BillingAPI.ViewModels;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CoinsController
{
    private readonly CoinEmissionValidator _coinEmissionValidator;
    private readonly Billing.BillingClient _grpcClient;
    private readonly ILogger<CoinsController> _logger;
    private readonly IMapper _mapper;
    private readonly MoveCoinsValidator _moveCoinsValidator;

    public CoinsController(Billing.BillingClient grpcClient,
        IMapper mapper,
        MoveCoinsValidator moveCoinsValidator,
        CoinEmissionValidator coinEmissionValidator,
        ILogger<CoinsController> logger)
    {
        _logger = logger;
        _mapper = mapper;
        _grpcClient = grpcClient;
        _moveCoinsValidator = moveCoinsValidator;
        _coinEmissionValidator = coinEmissionValidator;
    }

    [Produces("application/json")]
    [HttpPost(Name = "MoveCoins")]
    public async Task<ActionResult<ResponseModel>> Move(MoveCoins moveCoins)
    {
        var valResult = await _moveCoinsValidator.ValidateAsync(moveCoins);
        if (!valResult.IsValid) return new ValidationFailedResult(valResult);

        var response = await _grpcClient.MoveCoinsAsync(new MoveCoinsTransaction
        {
            SrcUser = moveCoins.SrcUser,
            DstUser = moveCoins.DstUser,
            Amount = moveCoins.Amount
        });

        if (response.Status == Response.Types.Status.Failed)
            return new OperationFailedResult(new ErrorModel("Operation failed", response.Comment));

        return _mapper.Map<ResponseModel>(response);
    }

    [Produces("application/json")]
    [HttpPost(Name = "CoinsEmission")]
    public async Task<ActionResult<ResponseModel>> Emission(CoinEmission coinEmission)
    {
        var valResult = await _coinEmissionValidator.ValidateAsync(coinEmission);
        if (!valResult.IsValid) return new ValidationFailedResult(valResult);
        var response = await _grpcClient.CoinsEmissionAsync(new EmissionAmount
        {
            Amount = coinEmission.Amount
        });
        if (response.Status == Response.Types.Status.Failed)
            return new OperationFailedResult(new ErrorModel("Operation failed", response.Comment));
        return _mapper.Map<ResponseModel>(response);
    }

    [Produces("application/json")]
    [HttpGet(Name = "LongestHistoryCoin")]
    public async Task<ActionResult<CoinModel>> LongestHistoryCoin()
    {
        try
        {
            var coin = await _grpcClient.LongestHistoryCoinAsync(new None());
            return _mapper.Map<CoinModel>(coin);
        }
        catch (RpcException ex)
        {
            if (ex.StatusCode == StatusCode.OutOfRange)
                return new OperationFailedResult(new ErrorModel("Out Of Range", ex.Status.Detail));
            throw;
        }
    }
}