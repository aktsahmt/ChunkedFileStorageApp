using ChunkedFileStorageApp.Core.Entities;

namespace ChunkedFileStorageApp.Core.Interfaces.Repository;

public interface IStoredFileRepository : IRepository<StoredFile>
{
    Task<StoredFile?> GetFileMetadataAsync(Guid fileId);
    Task SaveFileMetadataAsync(StoredFile file);
    Task<IEnumerable<StoredFile>> GetAllFilesAsync();
}