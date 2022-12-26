using BillingAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI;

public class OperationFailedResult : JsonResult
{
    private const int DefaultStatusCode = StatusCodes.Status400BadRequest;

    public OperationFailedResult(ErrorModel error) : base(error)
    {
        StatusCode = DefaultStatusCode;
    }
}