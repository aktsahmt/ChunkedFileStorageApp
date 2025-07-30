using ChunkedFileStorageApp.Application.Interfaces;
using ChunkedFileStorageApp.Core.Interfaces.Provider;

namespace ChunkedFileStorageApp.Application.Services;
public class RoundRobinStorageSelector : IRoundRobinStorageSelector
{
    private readonly IReadOnlyList<IStorageProvider> _providers;
    private int _currentIndex = -1;
    private readonly object _lock = new();

    public RoundRobinStorageSelector(IStorageProviderFactory factory)
    {
        _providers = factory.GetAllProviders();
    }

    public IStorageProvider GetNextProvider()
    {
        lock (_lock)
        {
            _currentIndex = (_currentIndex + 1) % _providers.Count;
            return _providers[_currentIndex];
        }
    }
}
