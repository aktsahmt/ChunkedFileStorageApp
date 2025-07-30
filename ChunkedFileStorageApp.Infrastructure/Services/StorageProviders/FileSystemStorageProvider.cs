using ChunkedFileStorageApp.Core.Common.Enums;
using ChunkedFileStorageApp.Core.Configurations;
using ChunkedFileStorageApp.Core.Interfaces.Provider;
using Microsoft.Extensions.Options;

namespace ChunkedFileStorageApp.Infrastructure.Services.StorageProviders;
public class FileSystemStorageProvider : IFileSystemStorageProvider
{
    private readonly ChunkSettings _chunkSettings;
    public string ProviderName => StorageProviderType.FileSystem.ToString();

    public FileSystemStorageProvider(IOptions<ChunkSettings> chunkSettings)
    {
        _chunkSettings = chunkSettings.Value;

        if (!Directory.Exists(_chunkSettings.StoragePath))
            Directory.CreateDirectory(_chunkSettings.StoragePath);
    }

    public async Task SaveChunkAsync(Guid chunkId, byte[] data)
    {
        string fullPath = Path.Combine(_chunkSettings.StoragePath, chunkId.ToString());

        await File.WriteAllBytesAsync(fullPath, data);
    }

    public async Task<byte[]> ReadChunkAsync(Guid chunkId)
    {
        string fullPath = Path.Combine(_chunkSettings.StoragePath, chunkId.ToString());

        return await File.ReadAllBytesAsync(fullPath);
    }
}