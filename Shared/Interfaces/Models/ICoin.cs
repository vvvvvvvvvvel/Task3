namespace Shared.Interfaces.Models;

public interface ICoin
{
    public long Id { get; }
    public IUser Owner { set; }
    public IEnumerable<string> GetHistory();
}