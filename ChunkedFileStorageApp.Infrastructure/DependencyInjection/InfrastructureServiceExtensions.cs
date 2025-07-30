using ChunkedFileStorageApp.Application.Interfaces;
using ChunkedFileStorageApp.Application.Services;
using ChunkedFileStorageApp.Core.Interfaces.Provider;
using ChunkedFileStorageApp.Core.Interfaces.Repository;
using ChunkedFileStorageApp.Core.Interfaces.Uow;
using ChunkedFileStorageApp.Infrastructure.Persistences;
using ChunkedFileStorageApp.Infrastructure.Repositories;
using ChunkedFileStorageApp.Infrastructure.Services.Factories;
using ChunkedFileStorageApp.Infrastructure.Services.StorageProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChunkedFileStorageApp.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<FileStorageDbContext>(opt => opt.UseInMemoryDatabase("FileStorageDb"));

        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<IStoredFileRepository, StoredFileRepository>();
        services.AddScoped<IStoredChunkDataRepository, StoredChunkDataRepository>();

        services.AddScoped<IStorageProvider, FileSystemStorageProvider>();
        services.AddScoped<IStorageProvider, DatabaseStorageProvider>();

        services.AddSingleton<IStorageProviderFactory, StorageProviderFactory>();
        services.AddSingleton<IRoundRobinStorageSelector, RoundRobinStorageSelector>();

        services.AddScoped<IFileChunkService, FileChunkService>();

        return services;
    }
}