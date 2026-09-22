using ContosoDashboard.Data;
using ContosoDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Services;

public class DocumentService
{
    private const long MaxFileSizeBytes = 25 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".ppt",
        ".pptx",
        ".txt",
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".bmp"
    };

    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/bmp"
    };

    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _storageService;
    private readonly INotificationService _notificationService;

    public DocumentService(ApplicationDbContext context, IFileStorageService storageService, INotificationService notificationService)
    {
        _context = context;
        _storageService = storageService;
        _notificationService = notificationService;
    }

    public async Task<DocumentValidationResult> ValidateUploadAsync(Stream fileStream, string fileName, string contentType, int userId, int? projectId, string role)
    {
        if (fileStream == null)
        {
            return DocumentValidationResult.Failure("A file stream is required.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return DocumentValidationResult.Failure("A file name is required.");
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return DocumentValidationResult.Failure("Unsupported file type. Allowed types are PDF, Office documents, text files, and common images.");
        }

        if (fileStream.Length <= 0)
        {
            return DocumentValidationResult.Failure("The uploaded file is empty.");
        }

        if (fileStream.Length > MaxFileSizeBytes)
        {
            return DocumentValidationResult.Failure("The uploaded file exceeds the 25 MB limit.");
        }

        if (!string.IsNullOrWhiteSpace(contentType) && !AllowedMimeTypes.Contains(contentType.Trim()))
        {
            return DocumentValidationResult.Failure("The uploaded file has an unsupported MIME type.");
        }

        if (projectId.HasValue)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId.Value);

            if (project == null)
            {
                return DocumentValidationResult.Failure("The selected project could not be found.");
            }

            var isProjectMember = project.ProjectMembers.Any(pm => pm.UserId == userId)
                || project.ProjectManagerId == userId;

            if (!isProjectMember && !IsAdministrator(role))
            {
                return DocumentValidationResult.Failure("You do not have access to upload to this project.");
            }
        }

        return DocumentValidationResult.Success();
    }

    public async Task<Document?> UploadDocumentAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string title,
        string? description,
        DocumentCategory category,
        string? tags,
        int uploadedByUserId,
        int? projectId,
        string role)
    {
        var validation = await ValidateUploadAsync(fileStream, fileName, contentType, uploadedByUserId, projectId, role);
        if (!validation.IsValid)
        {
            return null;
        }

        var safeFileName = Path.GetFileName(fileName);
        var relativePath = projectId.HasValue ? $"projects/{projectId.Value}" : $"users/{uploadedByUserId}";
        var storedName = $"{Guid.NewGuid():N}{Path.GetExtension(safeFileName)}";
        var filePath = await _storageService.UploadAsync(fileStream, safeFileName, contentType, relativePath);
        var document = new Document
        {
            Title = string.IsNullOrWhiteSpace(title) ? Path.GetFileNameWithoutExtension(safeFileName) : title.Trim(),
            Description = description,
            Category = category,
            Tags = tags,
            UploadedByUserId = uploadedByUserId,
            ProjectId = projectId,
            FileName = safeFileName,
            StoredFileName = storedName,
            FilePath = filePath,
            ContentType = contentType,
            FileSizeBytes = fileStream.Length,
            UploadedDate = DateTime.UtcNow,
            Status = DocumentStatus.PendingScan
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(new Notification
        {
            UserId = uploadedByUserId,
            Title = "Document uploaded",
            Message = $"Your document '{document.Title}' was uploaded successfully.",
            Type = NotificationType.SystemAnnouncement,
            Priority = NotificationPriority.Informational
        });

        return document;
    }

    public async Task<List<Document>> GetDocumentsForUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.ProjectMemberships)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return new List<Document>();
        }

        var projectIds = user.ProjectMemberships.Select(pm => pm.ProjectId).ToList();

        return await _context.Documents
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Where(d =>
                d.UploadedByUserId == userId ||
                (d.ProjectId.HasValue && projectIds.Contains(d.ProjectId.Value)) ||
                d.Shares.Any(s => s.SharedWithUserId == userId))
            .Where(d => !d.IsDeleted)
            .OrderByDescending(d => d.UploadedDate)
            .ToListAsync();
    }

    private static bool IsAdministrator(string role)
    {
        return string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase);
    }
}

public class DocumentValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static DocumentValidationResult Success() => new() { IsValid = true };

    public static DocumentValidationResult Failure(string message) => new() { IsValid = false, ErrorMessage = message };
}
