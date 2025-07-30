using ChunkedFileStorageApp.Core.Interfaces.Provider;

namespace ChunkedFileStorageApp.Application.Interfaces;
public interface IRoundRobinStorageSelector
{
    IStorageProvider GetNextProvider();
}