<!--
  Sync Impact Report
  ==================
  Version change: (new) → 1.0.0
  Modified principles: N/A (initial creation)
  Added sections:
    - Core Principles: I–VII
    - Technology Standards
    - Development Workflow
    - Governance
  Removed sections: None
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ no changes needed (generic Constitution Check)
    - .specify/templates/spec-template.md ✅ no changes needed (generic placeholders)
    - .specify/templates/tasks-template.md ✅ no changes needed (generic task format)
  Follow-up TODOs: None
-->

# AASHTOware Project Dashboard Constitution

## Core Principles

### I. OData-First API Design

All data endpoints MUST be exposed as OData entity sets using the
`Microsoft.AspNetCore.OData` library. Every queryable collection MUST
support `$filter`, `$select`, `$expand`, `$orderby`, `$top`, `$skip`,
and `$count`. Custom REST endpoints are permitted only when OData cannot
express the operation (e.g., batch actions, file uploads). Any deviation
MUST be justified in the Complexity Tracking section of the plan.

### II. Clean Architecture (Controllers → Data)

The project MUST follow a flat, folder-based separation:
- **Controllers/**: OData controllers inheriting from `ODataController`.
  One controller per entity type. Read-only (`Get`, `Get(key)`) unless
  write operations are explicitly scoped.
- **Models/**: Plain C# entity classes. No OData or EF attributes on
  model classes — configure via Fluent API and EDM builder.
- **Data/**: EF Core `DbContext` with Fluent API configuration for
  relationships, constraints, indexes, and seed data.
- **OData/**: EDM model builder. Centralized `ODataConventionModelBuilder`
  configuration in a dedicated static class.
- **Infrastructure/**: Cross-cutting middleware (error handling,
  validation, logging).

No repository pattern, no service layer, no CQRS unless the feature's
complexity explicitly demands it and is justified in Complexity Tracking.

### III. Integration Testing (NON-NEGOTIABLE)

Every OData controller MUST have integration tests using
`WebApplicationFactory<Program>` with EF Core in-memory database.
Integration tests MUST verify:
- Correct HTTP status codes (200, 404, 400)
- OData query options ($filter, $orderby, $top, $count) produce
  correct results
- Navigation property expansion ($expand) returns related data
- Invalid queries return structured OData error responses

Unit tests are OPTIONAL and reserved for non-trivial business logic
(e.g., EDM model configuration, custom validation).

### IV. Server-Side Pagination & Query Safety

All `[EnableQuery]` attributes MUST set `MaxTop = 1000` and
`PageSize = 100`. Unbounded queries MUST NOT be possible. The OData
library's built-in query validation MUST remain enabled — do not
disable `AllowedQueryOptions` or `AllowedFunctions` unless justified.

### V. Code-First Schema Management

Database schema MUST be managed via EF Core code-first migrations.
Raw SQL scripts in `src/sql/` serve as reference documentation for
DBA review but MUST NOT be the primary schema management mechanism.
Every migration MUST be generated from model changes, never hand-edited
unless correcting a migration bug.

### VI. Structured Error Handling

All API errors MUST return OData-formatted error responses (`ODataError`).
- 400: Invalid query syntax or parameters (handled by OData library)
- 404: Entity not found by key
- 500: Unhandled exceptions (custom middleware wraps in OData format)

Errors MUST include a `code` field (e.g., `InvalidQuery`,
`ResourceNotFound`, `InternalError`) and a human-readable `message`.
Stack traces MUST NOT leak to clients in non-Development environments.

### VII. Simplicity & YAGNI

Start with the minimum viable implementation. Specifically:
- No generic base controllers — each controller is explicit
- No service/repository layers until a second consumer exists
- No caching layer until measured performance requires it
- No authentication/authorization until identity provider integration
  is in scope
- Prefer convention over configuration; use
  `ODataConventionModelBuilder` unless explicit overrides are needed

## Technology Standards

**Runtime**: .NET 10 (ASP.NET Core)
**Language**: C# (latest stable language version)
**OData**: Microsoft.AspNetCore.OData 9.x (OData v8 protocol)
**ORM**: Entity Framework Core 10.x with SQL Server provider
**Database**: Microsoft SQL Server
**Testing**: xUnit + Microsoft.AspNetCore.Mvc.Testing + FluentAssertions
**API Documentation**: Swagger/OpenAPI (Swashbuckle) in Development

**.NET Coding Conventions**:
- Follow [Microsoft C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- PascalCase for public members, camelCase for locals/parameters
- `var` for obvious types, explicit types when clarity requires it
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- File-scoped namespaces
- Primary constructors where appropriate

**MSSQL Schema Guidelines**:
- Table names: PascalCase plural (Projects, ProjectItems)
- Column names: PascalCase (ProjectId, StartDate)
- Primary keys: `{EntityName}Id` as identity int
- Foreign keys: named `FK_{Child}_{Parent}` with cascade delete
- Indexes: named `IX_{Table}_{Columns}` on all foreign key columns
  and frequently filtered/sorted columns
- Decimal precision: `decimal(18,2)` for monetary, `decimal(18,4)`
  for quantities

## Development Workflow

**Branching**: Feature branches named `{NNN}-{short-name}` per Spec Kit
convention. All work against `main` branch.

**Commit Cadence**: Commit after each completed task or logical group.
Each commit MUST leave the project in a compilable state.

**Quality Gates**:
1. `dotnet build` MUST succeed with zero warnings (treat warnings as
   errors in CI)
2. `dotnet test` MUST pass all integration and unit tests
3. OData `$metadata` endpoint MUST return valid CSDL
4. Swagger UI MUST render all endpoints in Development environment

**Code Review Checklist**:
- [ ] No unbounded queries (MaxTop and PageSize set)
- [ ] Integration tests cover new/modified endpoints
- [ ] EF Core migration generated if model changed
- [ ] No secrets in committed files
- [ ] OData error responses follow structured format

## Governance

This constitution supersedes ad-hoc conventions for the AASHTOware
Project Dashboard codebase. All pull requests and code reviews MUST
verify compliance with these principles.

**Amendments**: Any change to this constitution requires:
1. Documentation of the proposed change
2. Justification in Complexity Tracking if it relaxes a principle
3. Version bump following semantic versioning (MAJOR for principle
   removal/redefinition, MINOR for new principles, PATCH for
   clarifications)
4. Update of `Last Amended` date

**Complexity Justification**: Any deviation from principles II or VII
(adding layers, patterns, or abstractions) MUST be documented in the
plan's Complexity Tracking table with: the violation, why it is needed,
and why the simpler alternative was rejected.

**Version**: 1.0.0 | **Ratified**: 2026-03-16 | **Last Amended**: 2026-03-16
