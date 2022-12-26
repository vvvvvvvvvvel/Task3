namespace Shared.Interfaces.Models;

public interface IResponse
{
    public enum Status
    {
        Unspecified,
        Ok,
        Failed
    }

    public Status ResponseStatus { get; init; }
    public string Comment { get; init; }

    public void Deconstruct(out Status responseStatus, out string comment)
    {
        responseStatus = ResponseStatus;
        comment = Comment;
    }
}