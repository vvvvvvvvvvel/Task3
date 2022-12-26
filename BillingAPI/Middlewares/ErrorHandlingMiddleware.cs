using BillingAPI.ViewModels;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;

namespace BillingAPI.Middlewares;

public static class ErrorHandlingMiddleware
{
    public static IApplicationBuilder UseBillingErrorHandling(
        this IApplicationBuilder builder)
    {
        builder.UseExceptionHandler(c => c.Run(Handle));
        return builder;
    }

    private static async Task Handle(HttpContext context)
    {
        var exception = context.Features
            .Get<IExceptionHandlerPathFeature>()!
            .Error;
        string responseDetail;
        if (exception is RpcException rpcException)
            responseDetail = rpcException.Status.Detail;
        else
            responseDetail = exception.Message;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ErrorModel("Internal Server Error", responseDetail));
    }
}