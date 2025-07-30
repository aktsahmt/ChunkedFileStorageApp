namespace ChunkedFileStorageApp.Core.Interfaces.Provider;
public interface IStorageProvider
{
    Task SaveChunkAsync(Guid chunkId, byte[] data);
    Task<byte[]> ReadChunkAsync(Guid chunkId);
    string ProviderName { get; }
}