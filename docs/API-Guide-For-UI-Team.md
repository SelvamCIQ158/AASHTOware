# AASHTOware Project Dashboard — API Guide for UI Team

**Version**: 1.0
**Date**: 2026-03-16
**Base URL**: `http://localhost:5000/odata`
**Swagger UI**: `http://localhost:5000/swagger`
**OData Metadata**: `http://localhost:5000/odata/$metadata`

---

## Table of Contents

1. [Overview](#overview)
2. [Entities & Fields](#entities--fields)
3. [Endpoints Reference](#endpoints-reference)
4. [OData Query Cheat Sheet](#odata-query-cheat-sheet)
5. [Dashboard Page → API Mapping](#dashboard-page--api-mapping)
6. [Response Format](#response-format)
7. [Error Handling](#error-handling)
8. [Seed Data for Development](#seed-data-for-development)
9. [FAQ](#faq)

---

## Overview

This is a **read-only OData v8 API** that serves all data for the Project Dashboard UI. There are no POST/PUT/DELETE operations — the API is query-only.

The API supports **4 entity types**:

| Entity | Endpoint | What It Represents |
|--------|----------|--------------------|
| Project | `/odata/Projects` | Highway/infrastructure construction projects |
| ProjectItem | `/odata/ProjectItems` | Bid items (line-item costs) within a project |
| FundingSource | `/odata/FundingSources` | Federal/state/local funding allocations |
| ValidationIssue | `/odata/ValidationIssues` | Data quality and compliance flags |

**All endpoints support** these OData query options:
- `$filter` — filter rows (like SQL WHERE)
- `$select` — choose which columns to return
- `$expand` — include related entities inline (like SQL JOIN)
- `$orderby` — sort results
- `$top` — limit number of results
- `$skip` — offset for pagination
- `$count` — include total record count

---

## Entities & Fields

### Project

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| ProjectId | int | No | Primary key |
| Name | string | No | Project name (max 200 chars) |
| Status | string | No | One of: `Active`, `Completed`, `OnHold`, `Draft`, `Cancelled` |
| StartDate | date | No | Project start date (format: `2025-06-01`) |
| EndDate | date | Yes | Project end date (null if not set) |
| TotalBudget | decimal | No | Total budget in USD (e.g., `45000000.00`) |
| Location | string | Yes | Location description (max 500 chars) |
| CreatedDate | datetime | No | Record creation timestamp (UTC) |
| ModifiedDate | datetime | No | Last modification timestamp (UTC) |

**Navigation properties** (accessible via `$expand`):
- `ProjectItems` — collection of bid items
- `FundingSources` — collection of funding sources
- `ValidationIssues` — collection of validation issues

---

### ProjectItem (Bid Item)

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| ProjectItemId | int | No | Primary key |
| ProjectId | int | No | Foreign key to Project |
| ItemNumber | string | No | AASHTO pay item code (e.g., `403-01`) |
| Description | string | No | Item description (e.g., "Hot Mix Asphalt Surface Course") |
| Quantity | decimal | No | Number of units (precision: 4 decimals) |
| Unit | string | No | Unit of measure: `TON`, `LF`, `SY`, `CY`, `EA`, `LS`, `SF`, `LB`, `AC`, `GAL` |
| UnitPrice | decimal | No | Price per unit (precision: 4 decimals) |
| Amount | decimal | No | Total = Quantity × UnitPrice (precision: 2 decimals) |
| CreatedDate | datetime | No | Record creation timestamp (UTC) |

**Navigation properties**: `Project`

---

### FundingSource

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| FundingSourceId | int | No | Primary key |
| ProjectId | int | No | Foreign key to Project |
| SourceName | string | No | Funding program name (e.g., "Federal Highway Administration (FHWA)") |
| FundingIdentifier | string | Yes | Grant/agreement number (e.g., `FHWA-2025-OR-0042`) |
| AllocatedAmount | decimal | No | Amount allocated from this source |
| CreatedDate | datetime | No | Record creation timestamp (UTC) |

**Navigation properties**: `Project`

**Common funding programs in seed data**:
- FHWA — Federal Highway Administration
- NHPP — National Highway Performance Program
- HSIP — Highway Safety Improvement Program
- STBG — Surface Transportation Block Grant
- State Transportation Fund
- Local Match (city/county)

---

### ValidationIssue

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| ValidationIssueId | int | No | Primary key |
| ProjectId | int | No | Foreign key to Project |
| Severity | string | No | One of: `Error`, `Warning`, `Info` |
| Description | string | No | Human-readable issue description |
| AffectedField | string | Yes | Which field/area is affected (e.g., `TotalBudget`, `EndDate`, `Status`) |
| DetectedDate | datetime | No | When the issue was detected (UTC) |

**Navigation properties**: `Project`

**Severity levels for UI styling**:
- `Error` — Red — must be resolved before project can proceed
- `Warning` — Yellow/Orange — review recommended
- `Info` — Blue/Gray — informational, no action required

---

## Endpoints Reference

### Projects

```http
# Get all projects
GET /odata/Projects

# Get single project by ID
GET /odata/Projects(1)

# Get project with all related data (for project detail page)
GET /odata/Projects(1)?$expand=ProjectItems,FundingSources,ValidationIssues

# Get only active projects
GET /odata/Projects?$filter=Status eq 'Active'

# Search by name (contains)
GET /odata/Projects?$filter=contains(Name, 'Highway')

# Get project count by status
GET /odata/Projects?$filter=Status eq 'Active'&$count=true&$top=0

# Paginated list (page 2, 20 per page)
GET /odata/Projects?$top=20&$skip=20&$count=true&$orderby=Name
```

### ProjectItems (Bid Items)

```http
# All bid items for a project
GET /odata/ProjectItems?$filter=ProjectId eq 1

# Top 10 most expensive items
GET /odata/ProjectItems?$filter=ProjectId eq 1&$orderby=Amount desc&$top=10

# With total count (for "showing 10 of 36 items")
GET /odata/ProjectItems?$filter=ProjectId eq 1&$orderby=Amount desc&$top=10&$count=true

# Only specific columns (lighter payload for grids)
GET /odata/ProjectItems?$filter=ProjectId eq 1&$select=ItemNumber,Description,Amount

# Paginated grid (page 2)
GET /odata/ProjectItems?$filter=ProjectId eq 1&$orderby=ItemNumber&$top=20&$skip=20&$count=true
```

### FundingSources

```http
# All funding sources for a project
GET /odata/FundingSources?$filter=ProjectId eq 1

# Sorted by amount (largest first)
GET /odata/FundingSources?$filter=ProjectId eq 1&$orderby=AllocatedAmount desc

# Only federal funding sources
GET /odata/FundingSources?$filter=ProjectId eq 1 and contains(SourceName, 'Federal')
```

### ValidationIssues

```http
# All issues for a project
GET /odata/ValidationIssues?$filter=ProjectId eq 1

# Only errors (critical issues)
GET /odata/ValidationIssues?$filter=ProjectId eq 1 and Severity eq 'Error'

# Sorted by severity (Errors first)
GET /odata/ValidationIssues?$filter=ProjectId eq 1&$orderby=Severity

# Count of errors for a badge/indicator
GET /odata/ValidationIssues?$filter=ProjectId eq 1 and Severity eq 'Error'&$count=true&$top=0
```

---

## OData Query Cheat Sheet

### Filtering ($filter)

| Operation | Syntax | Example |
|-----------|--------|---------|
| Equals | `eq` | `$filter=Status eq 'Active'` |
| Not equals | `ne` | `$filter=Status ne 'Cancelled'` |
| Greater than | `gt` | `$filter=TotalBudget gt 1000000` |
| Less than | `lt` | `$filter=Amount lt 500000` |
| Greater or equal | `ge` | `$filter=StartDate ge 2025-01-01` |
| Less or equal | `le` | `$filter=EndDate le 2027-12-31` |
| Contains | `contains()` | `$filter=contains(Name, 'Highway')` |
| Starts with | `startswith()` | `$filter=startswith(ItemNumber, '403')` |
| AND | `and` | `$filter=ProjectId eq 1 and Severity eq 'Error'` |
| OR | `or` | `$filter=Status eq 'Active' or Status eq 'Draft'` |
| NULL check | `eq null` | `$filter=EndDate eq null` |

### Selecting Fields ($select)

```
$select=ProjectId,Name,Status,TotalBudget
```

Reduces payload size — only return the fields you need.

### Expanding Relations ($expand)

```
# Single relation
$expand=ProjectItems

# Multiple relations
$expand=ProjectItems,FundingSources,ValidationIssues

# Expand with nested query
$expand=ProjectItems($orderby=Amount desc;$top=5)
```

### Sorting ($orderby)

```
$orderby=Name                    # ascending (default)
$orderby=Amount desc             # descending
$orderby=Severity,DetectedDate desc  # multi-column sort
```

### Pagination ($top, $skip, $count)

```
# First page (20 items)
$top=20&$skip=0&$count=true

# Second page
$top=20&$skip=20&$count=true

# Third page
$top=20&$skip=40&$count=true
```

The `$count=true` returns `@odata.count` in the response — use this for "Page X of Y" display.

**Server limits**: Max `$top` is 1000. Default page size is 100.

---

## Dashboard Page → API Mapping

### Project List Page

```
Purpose: Show all projects in a searchable, sortable table
```

| UI Feature | API Call |
|------------|----------|
| Load project list | `GET /odata/Projects?$top=20&$count=true&$orderby=Name` |
| Search by name | Add `&$filter=contains(Name, '{searchText}')` |
| Filter by status dropdown | Add `&$filter=Status eq '{status}'` |
| Sort by column | Change `$orderby` to clicked column |
| Pagination | Adjust `$skip` (page × pageSize) |
| Status badge counts | `GET /odata/Projects?$filter=Status eq 'Active'&$count=true&$top=0` (repeat per status) |

### Project Detail Page

```
Purpose: Show everything about one project
```

| UI Section | API Call |
|------------|----------|
| Load everything at once | `GET /odata/Projects({id})?$expand=ProjectItems,FundingSources,ValidationIssues` |
| **OR** load sections separately: | |
| Project header/overview | `GET /odata/Projects({id})` |
| Bid items grid | `GET /odata/ProjectItems?$filter=ProjectId eq {id}&$orderby=Amount desc&$count=true` |
| Funding sources panel | `GET /odata/FundingSources?$filter=ProjectId eq {id}&$orderby=AllocatedAmount desc` |
| Validation issues panel | `GET /odata/ValidationIssues?$filter=ProjectId eq {id}&$orderby=Severity` |
| Error count badge | `GET /odata/ValidationIssues?$filter=ProjectId eq {id} and Severity eq 'Error'&$count=true&$top=0` |

**Recommendation**: Use the single `$expand` call for initial load, then use individual endpoints for refreshing specific sections or paginated grids.

### Bid Items Grid

```
Purpose: Sortable, paginated grid of project line items
```

| UI Feature | API Call |
|------------|----------|
| Initial load | `GET /odata/ProjectItems?$filter=ProjectId eq {id}&$top=20&$count=true&$orderby=ItemNumber` |
| Sort by Amount | Change to `$orderby=Amount desc` |
| Sort by Item Number | Change to `$orderby=ItemNumber` |
| Next page | Increment `$skip` by 20 |
| Search items | Add `&$filter=... and contains(Description, '{search}')` |

---

## Response Format

### Collection Response

```json
{
  "@odata.context": "http://localhost:5000/odata/$metadata#Projects",
  "@odata.count": 5,
  "value": [
    {
      "ProjectId": 1,
      "Name": "Highway 101 Widening Phase 2",
      "Status": "Active",
      "StartDate": "2025-06-01",
      "EndDate": "2027-12-31",
      "TotalBudget": 45000000.00,
      "Location": "County Road 5, Mile Marker 12-18",
      "CreatedDate": "2026-01-15T10:30:00+00:00",
      "ModifiedDate": "2026-01-15T10:30:00+00:00"
    }
  ]
}
```

**Key points**:
- Collections are in the `value` array
- `@odata.count` only appears when you include `$count=true`
- Dates are ISO 8601 format

### Single Entity Response

```json
{
  "@odata.context": "http://localhost:5000/odata/$metadata#Projects/$entity",
  "ProjectId": 1,
  "Name": "Highway 101 Widening Phase 2",
  "Status": "Active",
  "StartDate": "2025-06-01",
  "EndDate": "2027-12-31",
  "TotalBudget": 45000000.00,
  "Location": "County Road 5, Mile Marker 12-18",
  "CreatedDate": "2026-01-15T10:30:00+00:00",
  "ModifiedDate": "2026-01-15T10:30:00+00:00"
}
```

**No `value` wrapper** — the entity is the root object.

### Expanded Response (with $expand)

```json
{
  "@odata.context": "...",
  "ProjectId": 1,
  "Name": "Highway 101 Widening Phase 2",
  "Status": "Active",
  "TotalBudget": 45000000.00,
  "ProjectItems": [
    {
      "ProjectItemId": 1,
      "ItemNumber": "403-01",
      "Description": "Hot Mix Asphalt Surface Course",
      "Amount": 1282500.00
    }
  ],
  "FundingSources": [
    {
      "FundingSourceId": 1,
      "SourceName": "Federal Highway Administration (FHWA)",
      "AllocatedAmount": 36000000.00
    }
  ],
  "ValidationIssues": [
    {
      "ValidationIssueId": 1,
      "Severity": "Warning",
      "Description": "Project end date is more than 2 years from start date."
    }
  ]
}
```

### Empty Collection

```json
{
  "@odata.context": "...",
  "value": []
}
```

---

## Error Handling

### 400 — Bad Request (Invalid Query)

Returned when the OData query syntax is wrong.

```json
{
  "error": {
    "code": "",
    "message": "The query specified in the URI is not valid. Could not find a property named 'InvalidField' on type 'Project'."
  }
}
```

**Common causes**:
- Filtering on a field that doesn't exist
- Wrong date format in filter
- Missing quotes around string values: use `Status eq 'Active'` not `Status eq Active`

### 404 — Not Found

Returned when requesting a single entity by ID that doesn't exist.

```
GET /odata/Projects(999999) → 404
```

### 500 — Internal Server Error

```json
{
  "error": {
    "code": "InternalError",
    "message": "An unexpected error occurred. Please try again later."
  }
}
```

---

## Seed Data for Development

The API ships with pre-loaded test data. No database setup needed for frontend development if using the in-memory test server.

### Projects (5)

| ID | Name | Status | Budget |
|----|------|--------|--------|
| 1 | Highway 101 Widening Phase 2 | Active | $45,000,000 |
| 2 | Interstate 5 Bridge Rehabilitation | Completed | $12,500,000 |
| 3 | Rural Highway Safety Improvements | OnHold | $3,200,000 |
| 4 | Downtown Arterial Resurfacing | Draft | $750,000 |
| 5 | Coastal Erosion Mitigation | Cancelled | $8,900,000 |

### ProjectItems (36 items across 5 projects)

Project 1 has 12 items, Project 2 has 8, Project 3 has 6, Project 4 has 5, Project 5 has 5.

Common item types: Hot Mix Asphalt, Structural Concrete, Steel Reinforcing, Guardrail, Drainage Pipe, Traffic Signal, Pavement Marking, Mobilization, Bridge Deck Repair, Erosion Control.

### FundingSources (12 across 5 projects)

Mix of FHWA, NHPP, HSIP, STBG, State Transportation Fund, Local Match, City/County funds.

### ValidationIssues (20 across 5 projects)

- 5 Errors (missing end dates, unexpended funds, unsigned agreements)
- 8 Warnings (budget variances, timeline concerns, missing documentation)
- 7 Info (reminders, closeout checklists, upcoming deadlines)

---

## FAQ

**Q: Do I need to set up a database for frontend development?**
A: No. The seed data is loaded via EF Core InMemory in the test environment. For full development, ask the backend team to run the SQL scripts or EF migrations.

**Q: What's the max number of records returned?**
A: Default page size is 100, maximum is 1000. Always use `$top` and `$skip` for pagination.

**Q: How do I get the total count for pagination?**
A: Add `$count=true` to your request. The count appears as `@odata.count` in the JSON response.

**Q: Can I filter by date range?**
A: Yes. Example: `$filter=StartDate ge 2025-01-01 and StartDate le 2025-12-31`

**Q: How do I handle the `@odata.context` field?**
A: Ignore it. It's OData metadata — not needed for UI rendering.

**Q: Can I make POST/PUT/DELETE requests?**
A: No. This API is read-only. Write operations are handled by a separate service.

**Q: What content type does the API return?**
A: `application/json; odata.metadata=minimal`

**Q: Are there rate limits?**
A: No rate limiting currently. Server-side pagination (max 1000) prevents excessive data transfer.

---

*Generated for the AASHTOware Project Dashboard. For API issues, contact the backend team. For interactive testing, use Swagger UI at `/swagger`.*
