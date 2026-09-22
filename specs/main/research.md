# Research: Document Upload and Management

## Decision

The feature will use a layered Blazor Server design that mirrors the existing dashboard architecture: EF Core for metadata persistence, the local filesystem for secure storage, and a storage abstraction interface to support future Azure migration without changing business logic.

## Rationale

- The repository already uses ASP.NET Core 8, Blazor Server, EF Core, and a service-oriented pattern, so the document feature should fit that shape without a large rewrite.
- The training requirement explicitly says files must be stored offline and outside wwwroot, which makes local filesystem storage with an abstraction boundary the safest and most aligned design.
- Existing user and project models already carry role and membership information that can support document authorization without introducing a new identity system.
- The app’s security posture depends on explicit authorization checks, so upload, download, share, and delete flows should be enforceable in services before any data or file action is executed.

## Alternatives considered

1. Store files directly in wwwroot
   - Rejected because it exposes uploaded files to browser access and weakens the security model.

2. Store everything in a single unstructured blob table without a service abstraction
   - Rejected because it would conflict with the repository’s existing separation of concerns and would make future migration harder.

3. Introduce a separate authorization system or role model for documents
   - Rejected because the feature requirements already align with current roles and project membership, and a separate model would add unnecessary complexity.

4. Use an external cloud storage description without local implementation
   - Rejected because the requirement states the feature must work offline and without cloud services during training.

## Key implementation decisions

- Document metadata will be stored in the application database with integer primary keys to match the existing User and Project model strategy.
- Category values will be stored as text strings to keep the editing and reporting model simple and readable.
- File paths will be generated before database insertion, using GUID-based names and project/user directory structures under an AppData/uploads path outside the web root.
- Sharing will be tracked through a DocumentShare entity so access decisions remain explicit and auditable.
- Audit events will be captured through a dedicated activity log instead of leaving activity only in UI history or ad hoc logs.

## Open questions resolved

- The feature uses local-only storage for the training implementation.
- The file storage interface will be designed as a repository-level abstraction with upload, delete, download, and URL generation methods.
- Access checks will be enforced in service logic and not only on the UI layer.
- The project remains offline-first and suitable for the existing training scenario.
