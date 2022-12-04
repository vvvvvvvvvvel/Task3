namespace Billing.Models;

public record ResponseViewModel(ResponseViewModel.Status ResponseStatus, string Comment)
{
    public enum Status
    {
        Unspecified,
        Ok,
        Failed
    }
}