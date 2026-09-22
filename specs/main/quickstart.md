# Quickstart Validation Guide

## Prerequisites

- .NET 8 SDK or later
- SQL Server LocalDB or equivalent local database support
- The ContosoDashboard app is available and running locally
- One of the seeded users is logged in via the mock authentication system

## Validation scenarios

### 1. Upload a valid document

1. Sign in as an employee or project manager.
2. Open the document area and choose Upload Document.
3. Select a valid PDF or image file under 25 MB.
4. Enter a title, choose a category, optionally add a project and tags, and submit.
5. Confirm that the file is accepted and appears in the user’s document list.

Expected outcome: the upload succeeds, the metadata is stored, and the document is listed with its file details.

### 2. Reject invalid uploads

1. Attempt to upload a file with an unsupported extension or a file larger than 25 MB.
2. Submit the form.

Expected outcome: the upload is rejected with a clear message and no row is created in the document metadata store.

### 3. Verify project visibility

1. Upload a file associated with a project.
2. Log in as another project team member.
3. Open the project details view.

Expected outcome: the file appears only for authorized project members and not for users outside the project.

### 4. Verify unauthorized access is denied

1. Obtain or infer a document URL for a document the current user should not access.
2. Attempt to navigate directly to the document or download route.

Expected outcome: access is denied and the user is prevented from downloading or viewing the file.

### 5. Share and review notifications

1. Upload a document and share it with another user.
2. Sign in as the recipient.
3. Open the Shared with Me view.

Expected outcome: the document appears in the recipient’s shared list and the notification is visible in the in-app notifications center.

### 6. Review audit trail

1. Upload a document and then download or delete it.
2. Open the audit or admin reporting view.

Expected outcome: the activity log shows the related upload, download, or deletion actions with the user and date.

## Expected results

- Document upload completes successfully for valid supported files.
- Invalid uploads fail safely and without orphaned database records.
- Team members see authorized documents, while unauthorized users are blocked.
- Shared documents remain visible only to permitted recipients.
- Audit records are available for compliance and review.
