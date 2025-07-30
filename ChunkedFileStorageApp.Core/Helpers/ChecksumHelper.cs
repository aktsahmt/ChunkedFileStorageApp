using System.Security.Cryptography;

namespace ChunkedFileStorageApp.Core.Helpers;

public static class ChecksumHelper
{
    public static string CalculateSHA256(byte[] data)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(data);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}