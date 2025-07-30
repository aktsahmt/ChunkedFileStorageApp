using ChunkedFileStorageApp.Core.Interfaces.Provider;

namespace ChunkedFileStorageApp.Infrastructure.Services.Factories;
public class StorageProviderFactory : IStorageProviderFactory
{
    private readonly IReadOnlyList<IStorageProvider> _providers;

    public StorageProviderFactory(IEnumerable<IStorageProvider> providers)
    {
        _providers = providers.ToList();
    }

    public IReadOnlyList<IStorageProvider> GetAllProviders()
    {
        return _providers;
    }
}
