namespace ChunkedFileStorageApp.Core.Configurations;

public class ChunkSettings
{
    public string StoragePath { get; set; } = string.Empty;
    public int MaxChunks { get; set; } = 100;
    public int MinChunkSizeMB { get; set; } = 1;
    public int MaxChunkSizeMB { get; set; } = 10;
}