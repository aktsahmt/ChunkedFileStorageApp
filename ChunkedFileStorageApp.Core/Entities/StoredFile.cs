namespace ChunkedFileStorageApp.Core.Entities;
public class StoredFile : BaseEntity
{
    public string OriginalFileName { get; set; } = null!;
    public long TotalSize { get; set; }
    public List<FileChunk> Chunks { get; set; } = [];
}
