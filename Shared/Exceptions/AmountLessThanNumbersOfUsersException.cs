namespace Shared.Exceptions;

public class AmountLessThanNumbersOfUsersException : Exception
{
    public AmountLessThanNumbersOfUsersException(string message = "Amount less than number of users") : base(message)
    {
    }
}