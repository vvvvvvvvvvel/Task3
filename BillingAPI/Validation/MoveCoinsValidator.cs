using BillingAPI.Models;
using FluentValidation;

namespace BillingAPI.Validation;

public class MoveCoinsValidator : AbstractValidator<MoveCoins>
{
    public MoveCoinsValidator()
    {
        RuleFor(x => x.SrcUser).NotEmpty();
        RuleFor(x => x.DstUser).NotEmpty();
        RuleFor(x => x.SrcUser).NotEqual(x => x.DstUser).WithMessage("Src User must not be equal to Dst User.");
        RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
    }
}