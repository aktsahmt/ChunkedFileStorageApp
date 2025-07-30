namespace ChunkedFileStorageApp.Core.Entities;

public class FileChunk : BaseEntity
{
    public Guid StoredFileId { get; set; }
    public int Order { get; set; }
    public long Size { get; set; }
    public string Checksum { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public string StorageProviderName { get; set; } = null!;
}