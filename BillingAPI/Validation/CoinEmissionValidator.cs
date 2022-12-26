using BillingAPI.Models;
using FluentValidation;

namespace BillingAPI.Validation;

public class CoinEmissionValidator : AbstractValidator<CoinEmission>
{
    public CoinEmissionValidator()
    {
        RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
    }
}