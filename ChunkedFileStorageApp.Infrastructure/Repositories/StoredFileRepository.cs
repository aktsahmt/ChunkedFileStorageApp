using ChunkedFileStorageApp.Core.Entities;
using ChunkedFileStorageApp.Core.Interfaces.Repository;
using ChunkedFileStorageApp.Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;

namespace ChunkedFileStorageApp.Infrastructure.Repositories;

public class StoredFileRepository : GenericRepository<StoredFile>, IStoredFileRepository
{
    public StoredFileRepository(FileStorageDbContext context) : base(context)
    {
    }

    public async Task SaveFileMetadataAsync(StoredFile file)
    {
        await _context.StoredFiles.AddAsync(file);
    }
    public async Task<StoredFile?> GetFileMetadataAsync(Guid fileId)
    {
        return await _context.StoredFiles
            .Include(f => f.Chunks)
            .FirstOrDefaultAsync(f => f.Id == fileId);
    }
    public async Task<IEnumerable<StoredFile>> GetAllFilesAsync()
    {
        return await _context.StoredFiles
            .Include(f => f.Chunks)
            .ToListAsync();
    }
}