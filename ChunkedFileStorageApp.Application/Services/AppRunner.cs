using ChunkedFileStorageApp.Application.Interfaces;
using ChunkedFileStorageApp.Core.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChunkedFileStorageApp.Application.Services;
public class AppRunner : IAppRunner
{
    private readonly IFileChunkService _fileService;
    private readonly IStoredFileRepository _fileRepository;
    private readonly ILogger<AppRunner> _logger;

    public AppRunner(IServiceProvider provider)
    {
        _fileService = provider.GetRequiredService<IFileChunkService>();
        _fileRepository = provider.GetRequiredService<IStoredFileRepository>();
        _logger = provider.GetRequiredService<ILogger<AppRunner>>();
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine("\n===== Distributed File Storage =====");
            Console.WriteLine("1. Tekli Dosya Yükle (Chunk + Storage)");
            Console.WriteLine("2. Çoklu Dosya Yükle (Chunk + Storage)");
            Console.WriteLine("3. Dosya Birleştir");
            Console.WriteLine("4. Tüm Dosyaları Listele");
            Console.WriteLine("5. Chunk Detayları");
            Console.WriteLine("0. Çıkış");
            Console.Write("Seçiminiz: ");

            var choice = Console.ReadLine();
            _logger.LogInformation("Kullanıcı seçimi: {Choice}", choice);

            switch (choice)
            {
                case "1":
                    await ProcessSingleFileUploadAsync();
                    break;
                case "2":
                    await ProcessMultipleFileUploadAsync();
                    break;
                case "3":
                    await ProcessMergeFilesAsync();
                    break;
                case "4":
                    await ProcessListAllFilesAsync();
                    break;
                case "5":
                    await ProcessChunkDetailsAsync();
                    break;
                case "0":
                    _logger.LogInformation("Uygulama kapatılıyor.");
                    return;
                default:
                    Console.WriteLine("Geçersiz seçim.");
                    _logger.LogWarning("Geçersiz seçim yapıldı: {Choice}", choice);
                    break;
            }
        }
    }

    private async Task ProcessChunkDetailsAsync()
    {
        Console.Write("Dosya ID: ");
        var id = Console.ReadLine();
        _logger.LogDebug("Chunk detayları istenen dosya ID: {Id}", id);

        try
        {
            var fileMeta = await _fileRepository.GetFileMetadataAsync(Guid.Parse(id!));
            if (fileMeta == null)
            {
                Console.WriteLine("Dosya bulunamadı.");
                _logger.LogWarning("Chunk detayları: Dosya bulunamadı. ID: {Id}", id);
                return;
            }

            foreach (var chunk in fileMeta.Chunks.OrderBy(c => c.Order))
            {
                Console.WriteLine($"Chunk {chunk.Order} | {chunk.Size / 1024} KB | Provider: {chunk.StorageProviderName}");
            }

            _logger.LogInformation("Dosya için {ChunkCount} adet chunk listelendi. ID: {FileId}", fileMeta.Chunks.Count, fileMeta.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            _logger.LogError(ex, "Chunk detayları alınırken hata oluştu. ID: {FileId}", id);
        }
        return;
    }
    private async Task ProcessListAllFilesAsync()
    {
        _logger.LogDebug("Tüm dosyalar listeleniyor...");
        var files = await _fileRepository.GetAllFilesAsync();
        Console.WriteLine("\n======= Dosyalar =======");

        if (files == null || !files.Any())
        {
            Console.WriteLine("Henüz yüklenmiş bir dosya bulunamadı.");
            _logger.LogInformation("Hiçbir dosya bulunamadı.");
            return;
        }

        foreach (var file in files)
        {
            Console.WriteLine($"{file.Id}- {file.OriginalFileName} ({file.TotalSize / 1024 / 1024:F2} MB) => {file.Chunks.Count} parça");
        }

        _logger.LogInformation("{Count} dosya listelendi.", files.Count());
        return;
    }
    private async Task ProcessMergeFilesAsync()
    {
        Console.Write("File ID: ");
        var fileIdToMerge = Console.ReadLine();
        Console.Write("Çıktı dosya adı: ");
        var outputPath = Console.ReadLine();

        _logger.LogDebug("Birleştirme başlatıldı. ID: {Id}, Output: {Output}", fileIdToMerge, outputPath);

        try
        {
            await _fileService.MergeChunksAsync(Guid.Parse(fileIdToMerge!), outputPath!);
            Console.WriteLine($"Dosya birleştirildi: {outputPath}");
            _logger.LogInformation("Dosya başarıyla birleştirildi: {Output}", outputPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            _logger.LogError(ex, "Dosya birleştirme sırasında hata oluştu. ID: {FileId}", fileIdToMerge);
        }
        return;
    }
    private async Task ProcessMultipleFileUploadAsync()
    {
        Console.WriteLine("Yüklemek istediğiniz dosyaların yollarını ; ile ayırarak giriniz:");
        Console.WriteLine(@"Örnek: C:\Dosyalar\resim1.jpg;C:\Dosyalar\pdf1.pdf");
        Console.Write("Yollar: ");
        var multiPathInput = Console.ReadLine();
        var paths = multiPathInput?.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(p => p.Trim())
                                   .Where(File.Exists)
                                   .ToList();

        if (paths == null || !paths.Any())
        {
            Console.WriteLine("Geçerli dosya bulunamadı.");
            _logger.LogWarning("Çoklu yükleme için geçerli dosya bulunamadı.");
            return;
        }

        try
        {
            var results = await _fileService.UploadMultipleFilesAsync(paths);
            Console.WriteLine($"{results.Count} dosya yüklendi.");
            foreach (var result in results)
            {
                Console.WriteLine($"Dosya yüklendi. File ID: {result}");
                _logger.LogInformation("Dosya yüklendi. ID: {FileId}", result);
            }
            _logger.LogInformation("{Count} dosya çoklu yüklemeyle yüklendi.", results.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            _logger.LogError(ex, "Çoklu dosya yükleme sırasında hata oluştu.");
        }
        return;
    }
    private async Task ProcessSingleFileUploadAsync()
    {
        Console.Write("Dosya yolu: ");
        var uploadPath = Console.ReadLine();
        _logger.LogDebug("Yükleme için girilen yol: {Path}", uploadPath);

        if (!File.Exists(uploadPath))
        {
            Console.WriteLine("Dosya bulunamadı.");
            _logger.LogWarning("Girilen dosya yolu geçersiz: {Path}", uploadPath);
            return;
        }

        try
        {
            var fileId = await _fileService.UploadFileAsync(uploadPath!);
            Console.WriteLine($"Dosya yüklendi. File ID: {fileId}");
            _logger.LogInformation("Dosya yüklendi. ID: {FileId}", fileId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            _logger.LogError(ex, "Dosya yükleme sırasında hata oluştu.");
        }
        return;
    }
}