namespace ChunkedFileStorageApp.Core.Interfaces.Provider;
public interface IStorageProviderFactory
{
    IReadOnlyList<IStorageProvider> GetAllProviders();
}
