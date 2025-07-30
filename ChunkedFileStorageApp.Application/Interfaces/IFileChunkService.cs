namespace ChunkedFileStorageApp.Application.Interfaces;
public interface IFileChunkService
{
    Task<Guid> UploadFileAsync(string filePath);
    Task MergeChunksAsync(Guid fileId, string outputPath);
    Task<List<Guid>> UploadMultipleFilesAsync(List<string> filePaths);
}