<!--
Sync Impact Report
- Version change: 0.0.0 -> 1.0.0
- Modified principles: n/a
- Added sections: Core Principles, Additional Constraints, Development Workflow, Governance
- Removed sections: template placeholders
- Deferred TODOs: none
-->

# ContosoDashboard Constitution

## Core Principles

### I. Security-First Training Design
The project MUST preserve the training-only security guardrails described by the repository documentation. Authentication, authorization, and data isolation MUST be explicit, reviewable, and aligned with the mock security model used in the application. No production-grade claims or deployments may be mistaken for actual production security controls; any change that weakens IDOR protections, role constraints, or cookie-based access boundaries MUST be treated as a governance issue and require review.

### II. Offline-First Architecture
The application MUST remain runnable without cloud services. Data access, file handling, and authentication abstractions MUST support local development and a clear migration path to Azure-managed services without rewriting core business logic. New features MUST prefer simple local implementations and clearly document when cloud-specific behavior is introduced.

### III. Test-First and Reviewable Change
Every non-trivial feature or behavior change MUST be supported by tests or explicit validation steps before merge. The team MUST write the failing scenario or expected behavior first, verify it fails or is missing, and then implement the smallest change that satisfies it. Code changes that alter security or access logic MUST be reviewed for implicit privilege changes and user isolation regressions.

### IV. Simplicity Over Cleverness
The codebase MUST favor clear separation of concerns, descriptive naming, and minimal abstractions. Business logic, UI concerns, and infrastructure access MUST remain easy to inspect and reason about. Complexity MUST be justified by user value or a concrete engineering requirement; otherwise, it is not allowed.

### V. Documentation and Context Preservation
Project decisions, architectural constraints, and training-specific caveats MUST be documented in the repository and preserved for future agents or contributors. When a change affects environment, security assumptions, or development workflow, the relevant docs and configuration files MUST be updated in the same change set.

## Additional Constraints
ContosoDashboard is training-focused and MUST NOT be treated as production-ready software. It MUST remain suitable for offline education, with mock identities and sample data instead of live external dependencies. Any feature that introduces external services, persistent secrets, or production identity management MUST be explicitly labeled as out-of-scope for the current training repository.

## Development Workflow
All feature work MUST begin from the repository’s documented architecture and security assumptions. Changes to data models, auth flows, page authorization, or service boundaries MUST be checked against the app’s training goals and its existing user-role model. Contributors MUST validate the affected behavior with the smallest relevant run or test flow and record the result in the PR or task notes. In the absence of automated tests, the team MUST document the verification steps and expected outcomes before merge.

## Governance
This Constitution supersedes ad-hoc local practice for this repository. Amendment requires a written proposal describing the change, the reason it is necessary, and any migration or rollout considerations. The project MUST maintain a single source of truth in the repository’s spec and documentation, and any change that affects security or architecture MUST be reviewed before merging. Compliance is satisfied when updates align with the principles above, and reviewers may reject a change if it weakens training safety, user isolation, security boundaries, or maintainability.

**Version**: 1.0.0 | **Ratified**: 2026-09-21 | **Last Amended**: 2026-09-21
