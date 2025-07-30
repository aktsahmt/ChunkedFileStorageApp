namespace ChunkedFileStorageApp.Core.Helpers;

public static class StringHelper
{
    public static string Truncate(string value, int maxLength)
    {
        return string.IsNullOrEmpty(value) ? value : value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }
}