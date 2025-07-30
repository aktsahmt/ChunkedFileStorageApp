using ChunkedFileStorageApp.Application.Interfaces;
using ChunkedFileStorageApp.Core.Configurations;
using ChunkedFileStorageApp.Core.Entities;
using ChunkedFileStorageApp.Core.Helpers;
using ChunkedFileStorageApp.Core.Interfaces.Provider;
using ChunkedFileStorageApp.Core.Interfaces.Repository;
using ChunkedFileStorageApp.Core.Interfaces.Uow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChunkedFileStorageApp.Application.Services;
public class FileChunkService : IFileChunkService
{
    private readonly IRoundRobinStorageSelector _roundRobinStorageSelector;
    private readonly IStorageProviderFactory _storageProviderFactory;
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FileChunkService> _logger;
    private readonly ChunkSettings _chunkSettings;

    public FileChunkService(
        IRoundRobinStorageSelector roundRobinStorageSelector,
        IStorageProviderFactory storageProviderFactory,
        IStoredFileRepository metadataRepository,
        ILogger<FileChunkService> logger,
        IOptions<ChunkSettings> chunkSettings,
        IUnitOfWork unitOfWork)
    {
        _roundRobinStorageSelector = roundRobinStorageSelector;
        _storageProviderFactory = storageProviderFactory;
        _storedFileRepository = metadataRepository;
        _logger = logger;
        _chunkSettings = chunkSettings.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> UploadFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var fileSize = fileBytes.Length;

        var estimatedChunkSizeMB = (int)Math.Ceiling((double)fileSize / _chunkSettings.MaxChunks / (1024 * 1024));
        var finalChunkSizeMB = Math.Clamp(estimatedChunkSizeMB, _chunkSettings.MinChunkSizeMB, _chunkSettings.MaxChunkSizeMB);
        var chunkSize = finalChunkSizeMB * 1024 * 1024;

        var totalChunks = (int)Math.Ceiling((double)fileSize / chunkSize);

        var storedFile = new StoredFile
        {
            OriginalFileName = Path.GetFileName(filePath),
            TotalSize = fileSize,
        };

        _logger.LogInformation("File will be split into {TotalChunks} chunks of approx {ChunkSizeMB} MB", totalChunks, finalChunkSizeMB);

        for (int i = 0; i < totalChunks; i++)
        {
            var chunkBytes = fileBytes.Skip(i * chunkSize).Take(chunkSize).ToArray();
            var chunkId = Guid.NewGuid();
            //var provider = GetProviderRoundRobin(i);
            var provider = _roundRobinStorageSelector.GetNextProvider();

            await provider.SaveChunkAsync(chunkId, chunkBytes);

            var checksum = ChecksumHelper.CalculateSHA256(chunkBytes);

            storedFile.Chunks.Add(new FileChunk
            {
                Id = chunkId,
                StoredFileId = storedFile.Id,
                Order = i,
                Size = chunkBytes.Length,
                StoragePath = chunkId.ToString(),
                StorageProviderName = provider.ProviderName,
                Checksum = checksum
            });

            _logger.LogInformation("Chunk {Index} stored by {ProviderName}", i, provider.ProviderName);
        }

        await _storedFileRepository.SaveFileMetadataAsync(storedFile);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Metadata saved for file {FileId}", storedFile.Id);

        return storedFile.Id;
    }

    public async Task<List<Guid>> UploadMultipleFilesAsync(List<string> filePaths)
    {
        var uploadedFileIds = new List<Guid>();

        foreach (var filePath in filePaths)
        {
            var fileId = await UploadFileAsync(filePath);
            uploadedFileIds.Add(fileId);
        }

        return uploadedFileIds;
    }

    public async Task MergeChunksAsync(Guid fileId, string outputPath)
    {
        var storedFile = await _storedFileRepository.GetFileMetadataAsync(fileId)
            ?? throw new InvalidOperationException("File metadata not found");

        var orderedChunks = storedFile.Chunks.OrderBy(c => c.Order).ToList();
        using var outputStream = new FileStream(outputPath, FileMode.Create);

        foreach (var chunk in orderedChunks)
        {
            var provider = _storageProviderFactory.GetAllProviders().FirstOrDefault(p => p.ProviderName == chunk.StorageProviderName)
                ?? throw new FileNotFoundException($"Provider {chunk.StorageProviderName} not found");

            var chunkBytes = await provider.ReadChunkAsync(chunk.Id);

            var calculatedChecksum = ChecksumHelper.CalculateSHA256(chunkBytes);
            if (calculatedChecksum != chunk.Checksum)
                throw new InvalidDataException($"Checksum mismatch for chunk {chunk.Id}");

            await outputStream.WriteAsync(chunkBytes);
            _logger.LogInformation("Chunk {ChunkId} merged", chunk.Id);
        }

        _logger.LogInformation("File merged to {OutputPath}", outputPath);
    }
}