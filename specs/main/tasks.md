# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/main/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and shared document infrastructure

- [X] T001 Create document feature directory structure under ContosoDashboard/ with model, service, and page entry points
- [ ] T002 [P] Add storage and document configuration defaults in ContosoDashboard/appsettings.json and appsettings.Development.json
- [X] T003 [P] Ensure the AppData/uploads directory exists for secure local file storage outside wwwroot
- [X] T004 [P] Add document-related dependency registration and service lifetime configuration in ContosoDashboard/Program.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that must be complete before any document user story is implemented

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

- [X] T005 Extend ContosoDashboard/Data/ApplicationDbContext.cs with document DbSets, relationships, indexes, and seed-ready configuration
- [X] T006 [P] Create ContosoDashboard/Models/Document.cs and document metadata model definitions matching the data-model contract
- [X] T007 [P] Create ContosoDashboard/Models/DocumentShare.cs and ContosoDashboard/Models/DocumentActivityLog.cs
- [X] T008 Create ContosoDashboard/Services/IFileStorageService.cs with upload, delete, download, and URL abstraction methods
- [X] T009 [P] Implement ContosoDashboard/Services/LocalFileStorageService.cs for offline local storage with GUID-based file names and path generation
- [X] T010 Add document authorization rules and upload validation helpers in the expected service layer in ContosoDashboard/Services/

---

## Phase 3: User Story 1 - Upload and organize work documents (Priority: P1) 🎯 MVP

**Goal**: Allow authenticated users to upload supported files, capture metadata, and see their documents in secure personal/project views.

**Independent Test**: A user can sign in, upload a valid PDF or image under 25 MB, complete title/category metadata, and confirm the document appears in the correct list without exposing it to unauthorized users.

### Implementation for User Story 1

- [X] T011 [P] [US1] Add Document validation and upload workflow methods to ContosoDashboard/Services/DocumentService.cs
- [X] T012 [P] [US1] Add the document metadata fields and access rules to ContosoDashboard/Models/Document.cs, including category values and required constraints
- [X] T013 [US1] Implement file validation in ContosoDashboard/Services/DocumentService.cs for extension whitelist, size limit, MIME validation, and secure path generation
- [ ] T014 [US1] Implement the upload flow in ContosoDashboard/Pages/DocumentUpload.razor or the appropriate page component for multi-file selection and metadata capture
- [ ] T015 [US1] Implement the user documents list and filtering in ContosoDashboard/Pages/Documents.razor or a document page set with category/project/date controls
- [ ] T016 [US1] Create or update project document display in ContosoDashboard/Pages/ProjectDetails.razor to show project-associated documents for authorized team members
- [X] T017 [US1] Add upload success/error messaging and progress feedback in the document page UI within ContosoDashboard/Pages/
- [X] T018 [US1] Add secure storage persistence and database record creation so the file is written before the metadata record is saved in ContosoDashboard/Services/

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently.

---

## Phase 4: User Story 2 - Share and access project documents securely (Priority: P1)

**Goal**: Ensure authorized users can view, download, and share documents while unauthorized users are blocked by service-layer checks.

**Independent Test**: A project member can view allowed docs, a document owner can share a file with another user, and a user without access cannot open the download path or direct file route.

### Implementation for User Story 2

- [ ] T019 [P] [US2] Add DocumentShare and recipient-access logic in ContosoDashboard/Models/DocumentShare.cs and ContosoDashboard/Services/DocumentService.cs
- [ ] T020 [US2] Implement permission checks for download, preview, update, and deletion in ContosoDashboard/Services/DocumentService.cs
- [ ] T021 [US2] Add shared-with-me and direct-access views in ContosoDashboard/Pages/ or a document management page set
- [ ] T022 [US2] Implement document preview/download authorization and file retrieval logic through the document service and storage abstraction in ContosoDashboard/Services/
- [ ] T023 [US2] Add sharing notifications and user-facing share actions in ContosoDashboard/Services/NotificationService.cs and the relevant document UI
- [ ] T024 [US2] Add delete confirmation and permanent removal workflow for owners and project managers in ContosoDashboard/Pages/ and ContosoDashboard/Services/

**Checkpoint**: At this point, User Stories 1 and 2 should both work independently.

---

## Phase 5: User Story 3 - Search, manage, and track document activity (Priority: P2)

**Goal**: Help users find documents quickly and give administrators audit visibility into document-related actions.

**Independent Test**: A user can search by title, description, tags, uploader, or project, and an administrator can review upload, share, download, and delete activity in the audit log.

### Implementation for User Story 3

- [ ] T025 [P] [US3] Implement document search and filter logic in ContosoDashboard/Services/DocumentService.cs using project, category, and date criteria
- [ ] T026 [US3] Add metadata edit and replace-file workflows in ContosoDashboard/Pages/ and ContosoDashboard/Services/DocumentService.cs
- [ ] T027 [US3] Create ContosoDashboard/Models/DocumentActivityLog.cs support and add audit insertion for upload, download, share, replace, and delete actions
- [ ] T028 [US3] Add admin/reporting views or reporting queries in ContosoDashboard/Pages/ or a reporting service for document usage and activity summaries
- [ ] T029 [US3] Ensure document search only returns records the current user is authorized to access in ContosoDashboard/Services/

**Checkpoint**: At this point, the core document lifecycle should be independently functional and auditable.

---

## Phase 6: User Story 4 - Surface document activity in everyday workflows (Priority: P3)

**Goal**: Integrate documents into task and dashboard workflows so recent file activity is visible without leaving the current work context.

**Independent Test**: A user can open the dashboard and task views and see recent documents relevant to them, or attach a document to a task while keeping the project association intact.

### Implementation for User Story 4

- [ ] T030 [P] [US4] Add Recent Documents widget data to ContosoDashboard/Services/DashboardService.cs and the dashboard summary card flow
- [ ] T031 [US4] Add document counts and recent document summary to the dashboard pages in ContosoDashboard/Pages/Index.razor
- [ ] T032 [US4] Add task document association and attach/upload actions in ContosoDashboard/Pages/Tasks.razor and task-related services
- [ ] T033 [US4] Ensure task-attached documents retain project association and permission checks in ContosoDashboard/Services/

**Checkpoint**: All document-related user stories should now be independently functional.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T034 [P] Add end-to-end validation and quickstart execution against ContosoDashboard/Pages/ and the feature flow described in specs/main/quickstart.md
- [ ] T035 [P] Review document and storage flows for security, IDOR protections, and authorization edge cases across ContosoDashboard/Services/ and ContosoDashboard/Pages/
- [ ] T036 [P] Add final documentation notes for the offline file storage pattern and cloud migration path in README.md or StakeholderDocs/
- [ ] T037 Run final build and smoke verification for the application using the relevant dotnet commands and confirm no regressions to the existing dashboard flows
- [ ] T038 Perform a last-pass cleanup of naming, comments, and validation messages across the document feature files before completion

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-6)**: All depend on Foundational completion and can proceed in priority order or in parallel where team capacity allows
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational; no dependency on other story work
- **User Story 2 (P1)**: Can start after Foundational; depends on the upload and storage model but should be independently testable
- **User Story 3 (P2)**: Can start after the core document lifecycle is in place
- **User Story 4 (P3)**: Can start after the core document features are working and should integrate with the existing dashboard/task flows

### Parallel Opportunities

- Setup tasks T002, T003, and T004 can run in parallel
- Foundational tasks T006, T007, and T009 can run in parallel once T005 and T008 are in place
- User Story 1 tasks T011, T012, and T014-T018 can proceed in parallel once foundational work is complete
- User Story 2 tasks T019, T021, and T023 can be worked in parallel with story-specific business logic updates
- User Story 3 and 4 work can proceed concurrently once core document operations are stable

---

## Parallel Example: User Story 1

```bash
# Parallel tasks once the foundational work is complete
Task: "Add Document validation and upload workflow methods in ContosoDashboard/Services/DocumentService.cs"
Task: "Create document metadata model definitions in ContosoDashboard/Models/Document.cs"
Task: "Implement document upload page and metadata capture in ContosoDashboard/Pages/"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. Stop and validate the upload flow independently
5. Deploy or demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → foundation ready
2. Add User Story 1 → test independently → demo (MVP)
3. Add User Story 2 → test independently → demo
4. Add User Story 3 → test independently → demo
5. Add User Story 4 → test independently → demo
6. Perform final polish and cross-cutting validation

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once foundational work is complete:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
   - Developer D: User Story 4
3. Final polish and validation after all stories are complete

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps each task to the relevant user story for traceability
- Each user story should be independently completable and testable
- Avoid vague tasks and file collisions
- Keep all access and validation actions in service-layer logic to preserve the application’s security model
