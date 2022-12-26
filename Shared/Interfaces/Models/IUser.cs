namespace Shared.Interfaces.Models;

public interface IUser
{
    public string Name { get; }
    public int Rating { get; }
    public long Amount { get; set; }
}