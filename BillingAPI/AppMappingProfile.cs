using AutoMapper;
using BillingAPI.ViewModels;

namespace BillingAPI;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<UserProfile, UserModel>();
        CreateMap<Coin, CoinModel>();
        CreateMap<Response, ResponseModel>().ConvertUsing((src, destination) =>
        {
            var status = src.Status switch
            {
                Response.Types.Status.Ok => ResponseModel.Status.Ok,
                Response.Types.Status.Failed => ResponseModel.Status.Failed,
                Response.Types.Status.Unspecified => ResponseModel.Status.Unspecified,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ResponseModel
            {
                ResponseStatus = status,
                Comment = src.Comment
            };
        });
    }
}