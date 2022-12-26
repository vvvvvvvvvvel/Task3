using AutoMapper;
using BillingAPI.ViewModels;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly Billing.BillingClient _grpcClient;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    public UsersController(Billing.BillingClient grpcClient, IMapper mapper, ILogger<UsersController> logger)
    {
        _logger = logger;
        _mapper = mapper;
        _grpcClient = grpcClient;
    }

    [HttpGet(Name = "ListUsers")]
    public async Task<ActionResult<List<UserModel>>> GetAll()
    {
        var users = new List<UserModel>();
        var listUsersResp = _grpcClient.ListUsers(new None());
        await foreach (var user in listUsersResp.ResponseStream.ReadAllAsync()) users.Add(_mapper.Map<UserModel>(user));
        return users;
    }
}