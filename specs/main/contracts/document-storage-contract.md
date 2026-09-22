# Document Storage Contract

## Overview

This contract defines the expected storage abstraction for uploaded files within the ContosoDashboard architecture. It is intended to keep business logic stable while allowing the training implementation to use local filesystem storage and a future Azure blob-backed implementation.

## Interface contract

```csharp
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string relativePath);
    Task DeleteAsync(string filePath);
    Task<Stream> DownloadAsync(string filePath);
    Task<string> GetUrlAsync(string filePath, TimeSpan expiration);
}
```

## Behavior requirements

- UploadAsync must return a safe storage path or identifier that can be persisted in the database.
- The implementation must reject invalid extensions, unsupported content types, and oversized files before the file is stored.
- Files must be saved outside the web root to reduce direct browser exposure.
- Generated file names must use GUID-based unique values to avoid collisions and path traversal hazards.
- DeleteAsync must remove the file the same way the stored path indicates, and must not leave broken metadata records behind.
- DownloadAsync must only return content for paths that the caller is authorized to access.

## Local implementation expectations

- The local implementation uses the file system and may store files under a structure such as AppData/uploads/{userId}/{projectId or personal}/{guid}.{ext}.
- Relative or portable paths are preferred so the business layer remains agnostic to storage mode.
- The same contract should support future Azure blob storage with the same method signatures.
