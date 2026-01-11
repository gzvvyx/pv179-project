using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Services;

public class FileService : IFileService
{
    private readonly string _baseStoragePath;
    private readonly ILogger<FileService> _logger;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _logger = logger;
        _baseStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        
        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
            _logger.LogInformation("Created base storage directory: {BasePath}", _baseStoragePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string? subdirectory = null)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));
        
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        var sanitizedFileName = SanitizeFileName(fileName);
        var uniqueFileName = GenerateUniqueFileName(sanitizedFileName);
        var directoryPath = GetDirectoryPath(subdirectory);
        var fullPath = Path.Combine(directoryPath, uniqueFileName);

        Directory.CreateDirectory(directoryPath);

        await using var fileStreamWriter = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fileStreamWriter);

        var relativePath = GetRelativePath(fullPath);
        _logger.LogInformation("File saved: {RelativePath}", relativePath);
        
        return relativePath;
    }

    public async Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string? subdirectory = null)
    {
        if (fileBytes == null)
            throw new ArgumentNullException(nameof(fileBytes));
        
        await using var stream = new MemoryStream(fileBytes);
        return await SaveFileAsync(stream, fileName, subdirectory);
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        var fullPath = GetFullPath(filePath);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
            return false;
        }

        try
        {
            await Task.Run(() => File.Delete(fullPath));
            _logger.LogInformation("File deleted: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            throw;
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.FromResult(false);

        var fullPath = GetFullPath(filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<Stream?> GetFileStreamAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.FromResult<Stream?>(null);

        var fullPath = GetFullPath(filePath);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("File not found: {FilePath}", filePath);
            return Task.FromResult<Stream?>(null);
        }

        try
        {
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<Stream?>(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
            return Task.FromResult<Stream?>(null);
        }
    }

    public string GetFullPath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        if (Path.IsPathRooted(filePath))
        {
            return filePath;
        }

        return Path.Combine(_baseStoragePath, filePath);
    }

    private string GetDirectoryPath(string? subdirectory)
    {
        return string.IsNullOrWhiteSpace(subdirectory)
            ? _baseStoragePath
            : Path.Combine(_baseStoragePath, subdirectory);
    }

    private string GetRelativePath(string fullPath)
    {
        var relativePath = Path.GetRelativePath(_baseStoragePath, fullPath);
        return relativePath.Replace(Path.DirectorySeparatorChar, '/');
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        
        sanitized = sanitized.Trim('.', ' ');
        
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            sanitized = "file";
        }
        
        return sanitized;
    }

    private static string GenerateUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var guid = Guid.NewGuid().ToString("N")[..8];
        
        return $"{nameWithoutExtension}_{timestamp}_{guid}{extension}";
    }
}

