using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Validation;

public class ValidationFailedResult : ObjectResult
{
    private const int DefaultStatusCode = StatusCodes.Status400BadRequest;

    public ValidationFailedResult(ValidationResult result) : base(null)
    {
        var desc = result.Errors.Select(e => $"{e.PropertyName} {e.ErrorMessage}");
        Value = new {error = "Validation failed", description = desc.ToArray()};
        StatusCode = DefaultStatusCode;
    }
}