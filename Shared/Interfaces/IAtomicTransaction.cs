namespace Shared.Interfaces;

public interface IAtomicTransaction : IDisposable, IAsyncDisposable
{
    public ValueTask CommitAsync();
}