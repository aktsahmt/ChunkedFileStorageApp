using ChunkedFileStorageApp.Core.Interfaces.Uow;
using ChunkedFileStorageApp.Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChunkedFileStorageApp.Infrastructure.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly FileStorageDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(FileStorageDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}