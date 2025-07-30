namespace ChunkedFileStorageApp.Core.Entities;
public class StoredChunkData : BaseEntity
{
    public byte[] Data { get; set; } = null!;
}
