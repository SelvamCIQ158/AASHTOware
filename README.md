# AASHTOware Project Dashboard OData API

A read-only OData v8 backend API for the AASHTOware Project Dashboard, built with ASP.NET Core (.NET 10), Entity Framework Core, and Microsoft SQL Server.

## Overview

This API serves project dashboard data for transportation agencies, exposing queryable OData endpoints for:

| Entity | Endpoint | Description |
|--------|----------|-------------|
| **Projects** | `/odata/Projects` | Construction/infrastructure projects with budget, status, dates, location |
| **ProjectItems** | `/odata/ProjectItems` | Bid items (line items) with AASHTO codes, quantities, unit prices |
| **FundingSources** | `/odata/FundingSources` | Federal/state/local funding allocations (FHWA, NHPP, HSIP, STBG) |
| **ValidationIssues** | `/odata/ValidationIssues` | Data quality and compliance issues with severity levels |

All endpoints support full OData query options: `$filter`, `$select`, `$expand`, `$orderby`, `$top`, `$skip`, `$count`.

## Tech Stack

- **Runtime**: .NET 10 / ASP.NET Core
- **OData**: Microsoft.AspNetCore.OData 9.x (OData v8)
- **ORM**: Entity Framework Core 10.x
- **Database**: Microsoft SQL Server
- **Docs**: Swagger/OpenAPI (Swashbuckle)
- **Testing**: xUnit + WebApplicationFactory + FluentAssertions

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full instance)

### Setup

```bash
# Clone and restore
git clone <repo-url>
cd AASHTOware
dotnet restore

# Apply database (update connection string in appsettings.json first)
cd src/AASHTOware.ProjectDashboard.Api
dotnet ef database update

# Run
dotnet run
```

The API will be available at `http://localhost:5000`.

### Alternative: SQL Scripts

If you prefer manual database setup:

```sql
-- Run in order against your SQL Server instance
src/sql/001_CreateTables.sql   -- Creates AASHTOware database + tables
src/sql/002_SeedData.sql       -- Populates with realistic sample data
```

## Example Queries

```
GET /odata/Projects(1)
GET /odata/Projects?$filter=Status eq 'Active'&$count=true
GET /odata/Projects(1)?$expand=ProjectItems,FundingSources,ValidationIssues
GET /odata/ProjectItems?$filter=ProjectId eq 1&$orderby=Amount desc&$top=10
GET /odata/FundingSources?$filter=ProjectId eq 1
GET /odata/ValidationIssues?$filter=ProjectId eq 1 and Severity eq 'Error'
```

## API Documentation

Swagger UI is available at `/swagger` when running in Development environment.

OData metadata: `/odata/$metadata`

## Project Structure

```
src/
├── AASHTOware.ProjectDashboard.Api/
│   ├── Controllers/          # OData controllers (one per entity)
│   ├── Models/               # Entity classes
│   ├── Data/                 # EF Core DbContext + seed data
│   ├── OData/                # EDM model configuration
│   ├── Infrastructure/       # Error handling middleware
│   └── Program.cs            # App configuration
├── AASHTOware.ProjectDashboard.Api.Tests/
│   ├── Integration/          # OData endpoint tests
│   └── Unit/                 # EDM model tests
└── sql/
    ├── 001_CreateTables.sql  # Database DDL
    └── 002_SeedData.sql      # Sample data (5 projects, 36+ bid items, 12 funding sources, 20 validation issues)
```

## Seed Data

The database ships with realistic transportation project data:

- **5 projects** across all statuses (Active, Completed, OnHold, Draft, Cancelled)
- **36+ bid items** using standard AASHTO pay item codes (Hot Mix Asphalt, Structural Concrete, Guardrail, Drainage Pipe, Traffic Signals, etc.)
- **12 funding sources** (FHWA, NHPP, HSIP, STBG, State/Local funds) with realistic federal grant IDs
- **20 validation issues** across Error, Warning, and Info severity levels

## Running Tests

```bash
dotnet test
```

27 integration and unit tests covering all OData query operations.

## Configuration

Connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ProjectDashboard": "Server=(localdb)\\mssqllocaldb;Database=AASHTOware;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## License

Proprietary - AASHTOware
