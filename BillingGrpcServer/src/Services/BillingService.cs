using BillingGrpcServer.Models;
using BillingGrpcServer.Utils;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Shared.Interfaces;
using Shared.Interfaces.Models;
using Shared.Interfaces.Services;

namespace BillingGrpcServer.Services;

public class BillingService<TUser, TCoin> : IBillingService
    where TUser : class, IUser
    where TCoin : class, ICoin
{
    private readonly IEmissionStrategy<TUser> _emissionStrategy;
    private readonly IUnitOfWork<TUser, TCoin> _unitOfWork;

    public BillingService(IEmissionStrategy<TUser> emissionStrategy,
        IUnitOfWork<TUser, TCoin> unitOfWork)
    {
        _emissionStrategy = emissionStrategy;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResponse> CoinsEmission(long amount)
    {
        try
        {
            var users = await _unitOfWork.UserRepository.GetAll().GetArray();
            var rewards = _emissionStrategy.CalcEmission(users, amount);
            for (var i = 0; i < users.Length; i++)
            {
                await _unitOfWork.CoinRepository.CreateAsync(users[i], rewards[i]);
                users[i].Amount += rewards[i];
            }

            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel(IResponse.Status.Ok, "Done");
        }
        catch (AmountLessThanNumbersOfUsersException ex)
        {
            return new ResponseModel(IResponse.Status.Failed, ex.Message);
        }
    }

    public async Task<IResponse> MoveCoins(string src, string dst, long amount)
    {
        var srcUser = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Name == src);
        var dstUser = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Name == dst);
        if (srcUser is not null && dstUser is not null)
            return await MoveCoins(srcUser, dstUser, amount);
        var nonExistUser = srcUser is null ? src : dst;
        return new ResponseModel(IResponse.Status.Failed,
            $"User {nonExistUser} not found");
    }

    public async Task<ICoin> LongestHistoryCoin()
    {
        var longestHistoryCoinViewModel = await _unitOfWork.CoinRepository.LongestHistoryCoinAsync();
        if (longestHistoryCoinViewModel is null) throw new InvalidOperationException("Empty coins collection");
        return longestHistoryCoinViewModel;
    }

    public async Task<IEnumerable<IUser>> ListUsers()
    {
        return await _unitOfWork.UserRepository.GetAll().GetArray();
    }

    private async Task<IResponse> MoveCoins(IUser srcUserViewModel, IUser dstUserViewModel, long amount)
    {
        if (srcUserViewModel.Amount < amount)
            return new ResponseModel(IResponse.Status.Failed,
                $"Insufficient coins {srcUserViewModel.Amount} < {amount}");

        await using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            var availableCoins = _unitOfWork.CoinRepository.GetByOwner(srcUserViewModel.Name)
                .Take((int) amount);
            foreach (var coin in availableCoins) coin.Owner = dstUserViewModel;
            srcUserViewModel.Amount -= amount;
            dstUserViewModel.Amount += amount;
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        return new ResponseModel(IResponse.Status.Ok,
            $"Done: {srcUserViewModel.Name} {srcUserViewModel.Amount}, {dstUserViewModel.Name} {dstUserViewModel.Amount}");
    }
}