using ChunkedFileStorageApp.Core.Entities;
using ChunkedFileStorageApp.Core.Interfaces.Repository;
using ChunkedFileStorageApp.Infrastructure.Persistences;

namespace ChunkedFileStorageApp.Infrastructure.Repositories;
public class StoredChunkDataRepository : GenericRepository<StoredChunkData>, IStoredChunkDataRepository
{
    public StoredChunkDataRepository(FileStorageDbContext context) : base(context)
    {
    }
}