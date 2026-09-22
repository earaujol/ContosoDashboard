using ContosoDashboard.Data;
using ContosoDashboard.Models;
using ContosoDashboard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace ContosoDashboard.Tests;

public class DocumentServiceValidationTests
{
    [Fact]
    public void ApplicationDbContext_CanBuildDocumentRelationships()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        context.Database.EnsureCreated();

        Assert.NotNull(context.Model.FindEntityType(typeof(DocumentShare)));
    }

    [Fact]
    public async Task ValidateUploadAsync_RejectsUnsupportedExtension()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var service = new DocumentService(context, new NullFileStorageService(), new NullNotificationService());
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        var result = await service.ValidateUploadAsync(stream, "malware.exe", "application/octet-stream", 1, null, "Employee");

        Assert.False(result.IsValid);
        Assert.Contains("unsupported", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateUploadAsync_RejectsOversizedFile()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var service = new DocumentService(context, new NullFileStorageService(), new NullNotificationService());
        var stream = new MemoryStream(new byte[26 * 1024 * 1024]);

        var result = await service.ValidateUploadAsync(stream, "report.pdf", "application/pdf", 1, null, "Employee");

        Assert.False(result.IsValid);
        Assert.Contains("25 MB", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class NullFileStorageService : IFileStorageService
    {
        public Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string relativePath)
            => Task.FromResult(relativePath);

        public Task DeleteAsync(string filePath) => Task.CompletedTask;

        public Task<Stream> DownloadAsync(string filePath) => Task.FromResult<Stream>(new MemoryStream());

        public Task<string> GetUrlAsync(string filePath, TimeSpan expiration) => Task.FromResult(filePath);
    }

    private sealed class NullNotificationService : INotificationService
    {
        public Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
            => Task.FromResult(new List<Notification>());

        public Task<Notification> CreateNotificationAsync(Notification notification)
            => Task.FromResult(notification);

        public Task<bool> MarkAsReadAsync(int notificationId, int requestingUserId)
            => Task.FromResult(true);

        public Task<int> GetUnreadCountAsync(int userId)
            => Task.FromResult(0);
    }
}
