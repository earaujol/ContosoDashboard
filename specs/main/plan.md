# Implementation Plan: Document Upload and Management

**Branch**: `main` | **Date**: 2026-09-21 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/main/spec.md`

## Summary

The feature adds secure, role-aware document upload, search, sharing, preview, deletion, and audit tracking to the ContosoDashboard application. The implementation will reuse the existing Blazor Server + EF Core + service-oriented architecture, add a storage abstraction for local files outside the web root, and keep all access decisions enforced by service-layer authorization to match the repository’s mock security model. A background malware-scan workflow will be introduced as an asynchronous job using Azure Functions with Queue Storage triggers so uploads can be validated after storage while preserving responsiveness in the UI.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: ASP.NET Core, Blazor Server, Entity Framework Core, SQL Server LocalDB, Bootstrap 5  
**Storage**: SQL Server LocalDB for metadata; local filesystem under AppData/uploads for secure file storage; Azure Queue Storage for async background scanning in the cloud-ready design  
**Testing**: Manual browser validation and dotnet build/test as the project evolves; no dedicated automated suite exists yet; queue-trigger integration testing should validate scan workflow and retry behavior  
**Target Platform**: Local Windows development environment with browser-based access for training scenarios; Azure Functions + Queue Storage for the cloud migration pattern  
**Project Type**: Web application (single Blazor Server project) with optional async background worker extension  
**Performance Goals**: upload up to 25 MB within 30 seconds; document list/search within 2 seconds for up to 500 documents; preview loads within 3 seconds; queue-based scan jobs processed asynchronously without blocking upload completion  
**Constraints**: offline-only training mode, no cloud storage dependency, role-based access enforcement, secure file handling outside wwwroot, support existing mock auth and project membership model, async queue-based scanning must not halt the user experience  
**Scale/Scope**: Small training application with a few seeded users, projects, tasks, and document workflows; future-ready for Azure-hosted scanning workloads

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Security-first training design: PASS. The plan keeps the application’s mock authorization model and adds explicit service checks around upload, preview, share, and delete operations.
- Offline-first architecture: PASS. The requirement remains local-only for the training version and uses an abstraction to support a future Azure replacement.
- Test-first and reviewable change: PASS with operational validation. New upload and permission behaviors should be exercised through browser flows and targeted build validation before merge.
- Simplicity over cleverness: PASS. The design keeps a single app structure, a storage abstraction, and clear service boundaries rather than introducing an unnecessary separate system.
- Documentation and context preservation: PASS. The feature will generate and maintain spec, research, model, contract, and quickstart artifacts in the feature directory.

## Project Structure

### Documentation (this feature)

```text
specs/main/
├── spec.md              # Feature requirements
├── plan.md              # This file
├── research.md          # Research findings and design choices
├── data-model.md        # Entity and data contract design
├── quickstart.md        # Validation scenarios
├── contracts/           # External/interfacing contracts
└── tasks.md             # Future implementation tasks (not yet created)
```

### Source Code (repository root)

```text
ContosoDashboard/
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── Document.cs
│   ├── DocumentShare.cs
│   ├── DocumentActivityLog.cs
│   └── existing domain models...
├── Services/
│   ├── DocumentService.cs
│   ├── IFileStorageService.cs
│   ├── LocalFileStorageService.cs
│   └── notification/auth/security helpers
├── Pages/
│   ├── Documents.razor
│   ├── DocumentUpload.razor
│   ├── ProjectDetails.razor (document widgets)
│   └── dashboard/task integrations
├── AppData/
│   └── uploads/
├── wwwroot/
│   └── static assets only
├── Program.cs
└── appsettings*.json
```

**Structure Decision**: Single Blazor Server application, with the new document functionality added to the existing Data, Models, Services, and Pages layers. In the cloud-ready design, upload processing can enqueue a file-scan request to an Azure Function triggered by Queue Storage; the UI remains unchanged while the scan job runs asynchronously in the background. This matches the repository structure and avoids unnecessary backend/frontend split for a training-focused app.

### Background scanning workflow

```text
Upload request -> DocumentService validates file metadata -> local file write completes -> queue message enqueued -> Azure Function Queue trigger -> file scanning job runs -> status updated to clean/quarantined -> notifications and audit log updated
```

The Azure Functions implementation should be optional and isolated from the offline training runtime. In the local training mode, the upload path may either skip the queue step or simulate it with a no-op handler, while the cloud migration design uses Azure Functions with Queue Storage triggers to process virus scan requests asynchronously after upload.

## Complexity Tracking

No constitution violations are required to justify. The design stays within the existing application pattern and keeps the feature scope limited to the training app’s offline-first requirements.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |
