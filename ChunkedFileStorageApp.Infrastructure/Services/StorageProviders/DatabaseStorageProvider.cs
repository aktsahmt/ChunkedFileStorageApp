using ChunkedFileStorageApp.Core.Common.Enums;
using ChunkedFileStorageApp.Core.Entities;
using ChunkedFileStorageApp.Core.Interfaces.Provider;
using ChunkedFileStorageApp.Core.Interfaces.Repository;

namespace ChunkedFileStorageApp.Infrastructure.Services.StorageProviders;
public class DatabaseStorageProvider : IDatabaseStorageProvider
{
    private readonly IStoredChunkDataRepository _storedChunkDataRepository;
    public string ProviderName => StorageProviderType.Database.ToString();

    public DatabaseStorageProvider(IStoredChunkDataRepository storedChunkDataRepository)
    {
        _storedChunkDataRepository = storedChunkDataRepository;
    }

    public async Task SaveChunkAsync(Guid chunkId, byte[] data)
    {
        await _storedChunkDataRepository.AddAsync(new StoredChunkData
        {
            Id = chunkId,
            Data = data
        });
    }

    public async Task<byte[]> ReadChunkAsync(Guid chunkId)
    {
        var chunk = await _storedChunkDataRepository.GetByIdAsync(chunkId);

        if (chunk == null)
            throw new FileNotFoundException($"Chunk {chunkId} not found in database");

        return chunk.Data;
    }
}