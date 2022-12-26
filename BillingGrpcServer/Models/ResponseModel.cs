using Shared.Interfaces.Models;

namespace BillingGrpcServer.Models;

public class ResponseModel : IResponse
{
    public ResponseModel(IResponse.Status responseStatus, string comment)
    {
        ResponseStatus = responseStatus;
        Comment = comment;
    }

    public IResponse.Status ResponseStatus { get; init; }
    public string Comment { get; init; }
}