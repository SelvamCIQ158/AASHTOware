namespace AASHTOware.ProjectDashboard.Api.Tests.Integration;

using AASHTOware.ProjectDashboard.Api.Data;
using AASHTOware.ProjectDashboard.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class ProjectDashboardWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ProjectDashboardDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Also remove any DbContext-related registrations that reference SQL Server
            var dbContextDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("DbContext") == true
                         || d.ServiceType.FullName?.Contains("SqlServer") == true)
                .ToList();

            foreach (var d in dbContextDescriptors)
            {
                services.Remove(d);
            }

            // Add InMemory database
            services.AddDbContext<ProjectDashboardDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Build a service provider and seed the database manually.
            // Do NOT call EnsureCreated() because the InMemory provider would
            // execute the HasData seed from OnModelCreating, which conflicts
            // with the controlled test data we want.
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProjectDashboardDbContext>();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(ProjectDashboardDbContext db)
    {
        if (db.Projects.Any())
        {
            return;
        }

        var now = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var detectDate = new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc);

        // Projects
        var project1 = new Project
        {
            ProjectId = 1,
            Name = "Highway 101 Widening Phase 2",
            Status = "Active",
            StartDate = new DateOnly(2025, 6, 1),
            EndDate = new DateOnly(2027, 12, 31),
            TotalBudget = 45_000_000.00m,
            Location = "County Road 5, Mile Marker 12-18",
            CreatedDate = now,
            ModifiedDate = now
        };

        var project2 = new Project
        {
            ProjectId = 2,
            Name = "Interstate 5 Bridge Rehabilitation",
            Status = "Completed",
            StartDate = new DateOnly(2023, 3, 15),
            EndDate = new DateOnly(2025, 9, 30),
            TotalBudget = 12_500_000.00m,
            Location = "I-5 Overpass at River Mile 42",
            CreatedDate = now,
            ModifiedDate = now
        };

        var project3 = new Project
        {
            ProjectId = 3,
            Name = "Rural Highway Safety Improvements",
            Status = "Active",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2027, 6, 30),
            TotalBudget = 3_200_000.00m,
            Location = "State Route 22, MP 5-15",
            CreatedDate = now,
            ModifiedDate = now
        };

        db.Projects.AddRange(project1, project2, project3);

        // Project Items
        db.ProjectItems.AddRange(
            new ProjectItem
            {
                ProjectItemId = 1, ProjectId = 1, ItemNumber = "403-01",
                Description = "Hot Mix Asphalt Surface Course",
                Quantity = 15000m, Unit = "TON", UnitPrice = 85.50m,
                Amount = 1_282_500.00m, CreatedDate = now
            },
            new ProjectItem
            {
                ProjectItemId = 2, ProjectId = 1, ItemNumber = "403-02",
                Description = "Hot Mix Asphalt Base Course",
                Quantity = 25000m, Unit = "TON", UnitPrice = 72.00m,
                Amount = 1_800_000.00m, CreatedDate = now
            },
            new ProjectItem
            {
                ProjectItemId = 3, ProjectId = 1, ItemNumber = "501-01",
                Description = "Structural Concrete Class 4000",
                Quantity = 3500m, Unit = "CY", UnitPrice = 650.00m,
                Amount = 2_275_000.00m, CreatedDate = now
            },
            new ProjectItem
            {
                ProjectItemId = 4, ProjectId = 2, ItemNumber = "502-01",
                Description = "Bridge Deck Repair",
                Quantity = 8500m, Unit = "SF", UnitPrice = 185.00m,
                Amount = 1_572_500.00m, CreatedDate = now
            },
            new ProjectItem
            {
                ProjectItemId = 5, ProjectId = 2, ItemNumber = "502-02",
                Description = "Bridge Deck Overlay Polymer",
                Quantity = 8500m, Unit = "SF", UnitPrice = 45.00m,
                Amount = 382_500.00m, CreatedDate = now
            }
        );

        // Funding Sources
        db.FundingSources.AddRange(
            new FundingSource
            {
                FundingSourceId = 1, ProjectId = 1,
                SourceName = "Federal Highway Administration (FHWA)",
                FundingIdentifier = "FHWA-2025-OR-0042",
                AllocatedAmount = 36_000_000.00m, CreatedDate = now
            },
            new FundingSource
            {
                FundingSourceId = 2, ProjectId = 1,
                SourceName = "State Transportation Fund",
                FundingIdentifier = "STF-2025-118",
                AllocatedAmount = 9_000_000.00m, CreatedDate = now
            },
            new FundingSource
            {
                FundingSourceId = 3, ProjectId = 2,
                SourceName = "National Highway Performance Program (NHPP)",
                FundingIdentifier = "NHPP-2023-OR-0188",
                AllocatedAmount = 10_000_000.00m, CreatedDate = now
            }
        );

        // Validation Issues
        db.ValidationIssues.AddRange(
            new ValidationIssue
            {
                ValidationIssueId = 1, ProjectId = 1, Severity = "Warning",
                Description = "Project end date is more than 2 years from start date.",
                AffectedField = "EndDate", DetectedDate = detectDate
            },
            new ValidationIssue
            {
                ValidationIssueId = 2, ProjectId = 1, Severity = "Error",
                Description = "DBE participation goal not set.",
                AffectedField = null, DetectedDate = detectDate
            },
            new ValidationIssue
            {
                ValidationIssueId = 3, ProjectId = 1, Severity = "Info",
                Description = "Federal funding requires annual progress report.",
                AffectedField = null, DetectedDate = detectDate
            },
            new ValidationIssue
            {
                ValidationIssueId = 4, ProjectId = 2, Severity = "Info",
                Description = "Project marked as Completed. Final inspection pending.",
                AffectedField = "Status", DetectedDate = detectDate
            },
            new ValidationIssue
            {
                ValidationIssueId = 5, ProjectId = 2, Severity = "Warning",
                Description = "Final cost exceeds original estimate by 8.2%.",
                AffectedField = "TotalBudget", DetectedDate = detectDate
            },
            new ValidationIssue
            {
                ValidationIssueId = 6, ProjectId = 3, Severity = "Error",
                Description = "Project status is OnHold but no hold reason documented.",
                AffectedField = "Status", DetectedDate = detectDate
            }
        );

        db.SaveChanges();
    }
}
