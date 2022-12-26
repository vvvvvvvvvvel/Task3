using Shared.Interfaces;

namespace Local;

public class AtomicTransaction : IAtomicTransaction
{
    private readonly object _syncObj;

    private bool _disposedValue;

    public AtomicTransaction(object syncObj)
    {
        _syncObj = syncObj;
        var enter = false;
        Monitor.TryEnter(_syncObj, 1000, ref enter);
        if (!enter) throw new Exception("Monitor can`t enter");
    }

    public void Dispose()
    {
        Dispose(true);
    }


    public ValueTask DisposeAsync()
    {
        Dispose(true);
        return ValueTask.CompletedTask;
    }

    public ValueTask CommitAsync()
    {
        //pass
        return ValueTask.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing) Monitor.Exit(_syncObj);

        _disposedValue = true;
    }
}