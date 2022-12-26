namespace BillingAPI.Models;

public record MoveCoins(string SrcUser, string DstUser, long Amount);