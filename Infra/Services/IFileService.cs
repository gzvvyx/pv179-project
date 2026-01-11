namespace Infra.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string? subdirectory = null);
    Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string? subdirectory = null);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    Task<Stream?> GetFileStreamAsync(string filePath);
    string GetFullPath(string filePath);
}

