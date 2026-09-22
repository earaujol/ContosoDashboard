# Data Model: Document Upload and Management

## Entities

### Document

Represents a stored document and its metadata.

| Field | Type | Notes |
|---|---|---|
| DocumentId | int | Primary key; consistent with existing project/user integer keys |
| Title | string | Required; short human-readable title |
| Description | string | Optional; summary or context |
| Category | string | Required; one of the predefined categories |
| FileName | string | Original file name for display and compatibility |
| StoredFileName | string | GUID-based safe file name stored on disk |
| FilePath | string | Relative or local storage path for retrieval |
| FileType | string | MIME type, max 255 characters to accommodate Office documents |
| FileSizeBytes | long | Tracks actual file size |
| UploadDateUtc | DateTime | Capture upload time |
| UploadedByUserId | int | Foreign key to user |
| ProjectId | int? | Optional project association |
| TaskId | int? | Optional task attachment |
| IsDeleted | bool | Supports soft delete or audit retention behavior |

Relationships:
- Many documents belong to one uploader
- Many documents may belong to one project
- Many documents may belong to one task
- A document may have many share records
- A document may have many activity records

### User

Existing user model continues to provide identity and authorization context.

| Field | Type | Notes |
|---|---|---|
| UserId | int | Existing key |
| DisplayName | string | Used in search and share notifications |
| Email | string | Existing login and identity field |
| Role | enum | Employee, TeamLead, ProjectManager, Administrator |
| Department | string | Existing metadata |

### Project

Existing project model remains the project context for document visibility and ownership.

| Field | Type | Notes |
|---|---|---|
| ProjectId | int | Existing key |
| Name | string | Project name |
| ProjectManagerId | int | Owner/manager of project access rules |
| Status | enum | Active, Planning, OnHold, Completed |

### DocumentShare

Tracks explicit share relationships beyond project-level access.

| Field | Type | Notes |
|---|---|---|
| DocumentShareId | int | Primary key |
| DocumentId | int | Shared document |
| SharedWithUserId | int | Direct recipient |
| SharedByUserId | int | Owner or authorized sharer |
| SharedDateUtc | DateTime | Share timestamp |
| Message | string | Optional sharing note |

### DocumentActivityLog

Records audit and operational activity.

| Field | Type | Notes |
|---|---|---|
| ActivityId | int | Primary key |
| DocumentId | int | Related document |
| UserId | int | Acting user |
| ActivityType | string | Upload, Download, Delete, Share, Replace, Preview |
| ActivityDateUtc | DateTime | Time of activity |
| Details | string | Summary or contextual note |

## Validation rules

- Title is required and must be non-empty
- Category must be one of the allowed values: Project Documents, Team Resources, Personal Files, Reports, Presentations, Other
- File type must be supported and validated before persistence
- File size must not exceed 25 MB per file
- FilePath must be generated before database insert to prevent duplicate key and orphaning issues
- Access to document records is not determined by the UI alone; service-level authorization rules are mandatory

## State transitions

Document lifecycle is based on business permissions rather than only a strict enum:

- Draft/Ready for upload -> Uploaded
- Uploaded -> Shared
- Uploaded -> Replaced
- Uploaded -> Deleted
- Shared -> Downloaded/Previewed
- Deleted -> Archived or removed from active queries

## Key relationships

- User 1..* Document
- Project 1..* Document
- Document 1..* DocumentShare
- Document 1..* DocumentActivityLog
- Document *..1 Task (optional association)
