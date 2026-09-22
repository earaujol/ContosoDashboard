using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DocumentCategory Category { get; set; } = DocumentCategory.General;

    [MaxLength(500)]
    public string? Tags { get; set; }

    [Required]
    public int UploadedByUserId { get; set; }

    public int? ProjectId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastModifiedDate { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.PendingScan;

    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(UploadedByUserId))]
    public virtual User UploadedByUser { get; set; } = null!;

    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }

    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
    public virtual ICollection<DocumentActivityLog> ActivityLogs { get; set; } = new List<DocumentActivityLog>();
}

public enum DocumentCategory
{
    General,
    Contract,
    Design,
    Specification,
    Report,
    Invoice,
    Image,
    Other
}

public enum DocumentStatus
{
    PendingScan,
    Approved,
    Quarantined,
    Rejected,
    Deleted
}
