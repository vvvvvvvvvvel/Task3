using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Interfaces;

namespace DbPostgres;

public class AtomicTransaction : IAtomicTransaction
{
    private readonly DatabaseFacade _db;

    private bool _disposedValue;
    private IDbContextTransaction _transaction;

    private AtomicTransaction(DatabaseFacade db)
    {
        _db = db;
    }

    public void Dispose()
    {
        Dispose(true);
    }


    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }

    public async ValueTask CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    private async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _transaction ??= await _db.BeginTransactionAsync();
    }


    public static async Task<AtomicTransaction> CreateAsync(DatabaseFacade db)
    {
        var transaction = new AtomicTransaction(db);
        await transaction.BeginTransactionAsync();
        return transaction;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing) _transaction.Dispose();

        _disposedValue = true;
    }
}