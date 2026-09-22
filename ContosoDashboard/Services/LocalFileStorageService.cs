namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService()
    {
        _basePath = Path.Combine(AppContext.BaseDirectory, "AppData", "uploads");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string relativePath)
    {
        if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.", nameof(fileName));

        var safeFileName = Path.GetFileName(fileName);
        var finalPath = BuildStoragePath(relativePath, safeFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

        await using var destination = File.Create(finalPath);
        await fileStream.CopyToAsync(destination);

        return finalPath;
    }

    public Task DeleteAsync(string filePath)
    {
        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    public Task<Stream> DownloadAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new FileNotFoundException("No file path was supplied.");
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The requested document was not found.", filePath);
        }

        var stream = File.OpenRead(filePath);
        return Task.FromResult<Stream>(stream);
    }

    public Task<string> GetUrlAsync(string filePath, TimeSpan expiration)
    {
        return Task.FromResult(filePath);
    }

    private string BuildStoragePath(string relativePath, string fileName)
    {
        var basePath = Path.GetFullPath(_basePath);
        var sanitizedRelativePath = relativePath
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        var relativeDirectory = string.IsNullOrWhiteSpace(sanitizedRelativePath)
            ? string.Empty
            : sanitizedRelativePath;

        var targetDirectory = Path.Combine(basePath, relativeDirectory);
        var finalName = Guid.NewGuid() + Path.GetExtension(fileName);
        return Path.Combine(targetDirectory, finalName);
    }
}
