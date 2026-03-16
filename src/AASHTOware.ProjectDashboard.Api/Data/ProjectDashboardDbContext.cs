using AASHTOware.ProjectDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AASHTOware.ProjectDashboard.Api.Data;

public class ProjectDashboardDbContext(DbContextOptions<ProjectDashboardDbContext> options)
    : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectItem> ProjectItems => Set<ProjectItem>();
    public DbSet<FundingSource> FundingSources => Set<FundingSource>();
    public DbSet<ValidationIssue> ValidationIssues => Set<ValidationIssue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureProject(modelBuilder);
        ConfigureProjectItem(modelBuilder);
        ConfigureFundingSource(modelBuilder);
        ConfigureValidationIssue(modelBuilder);
        SeedData(modelBuilder);
    }

    private static void ConfigureProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.TotalBudget).HasPrecision(18, 2);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(e => e.ProjectItems)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.FundingSources)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ValidationIssues)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProjectItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectItem>(entity =>
        {
            entity.HasKey(e => e.ProjectItemId);
            entity.Property(e => e.ItemNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => e.ProjectId).HasDatabaseName("IX_ProjectItems_ProjectId");
            entity.HasIndex(e => new { e.ProjectId, e.Amount })
                .HasDatabaseName("IX_ProjectItems_Amount")
                .IsDescending(false, true);
        });
    }

    private static void ConfigureFundingSource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FundingSource>(entity =>
        {
            entity.HasKey(e => e.FundingSourceId);
            entity.Property(e => e.SourceName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FundingIdentifier).HasMaxLength(100);
            entity.Property(e => e.AllocatedAmount).HasPrecision(18, 2);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => e.ProjectId).HasDatabaseName("IX_FundingSources_ProjectId");
        });
    }

    private static void ConfigureValidationIssue(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ValidationIssue>(entity =>
        {
            entity.HasKey(e => e.ValidationIssueId);
            entity.Property(e => e.Severity).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.AffectedField).HasMaxLength(200);
            entity.Property(e => e.DetectedDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => e.ProjectId).HasDatabaseName("IX_ValidationIssues_ProjectId");
            entity.HasIndex(e => new { e.ProjectId, e.Severity })
                .HasDatabaseName("IX_ValidationIssues_Severity");
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var now = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // --- Projects ---
        modelBuilder.Entity<Project>().HasData(
            new Project
            {
                ProjectId = 1, Name = "Highway 101 Widening Phase 2", Status = "Active",
                StartDate = new DateOnly(2025, 6, 1), EndDate = new DateOnly(2027, 12, 31),
                TotalBudget = 45_000_000.00m, Location = "County Road 5, Mile Marker 12-18",
                CreatedDate = now, ModifiedDate = now
            },
            new Project
            {
                ProjectId = 2, Name = "Interstate 5 Bridge Rehabilitation", Status = "Completed",
                StartDate = new DateOnly(2023, 3, 15), EndDate = new DateOnly(2025, 9, 30),
                TotalBudget = 12_500_000.00m, Location = "I-5 Overpass at River Mile 42",
                CreatedDate = now, ModifiedDate = now
            },
            new Project
            {
                ProjectId = 3, Name = "Rural Highway Safety Improvements", Status = "OnHold",
                StartDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2027, 6, 30),
                TotalBudget = 3_200_000.00m, Location = "State Route 22, MP 5-15",
                CreatedDate = now, ModifiedDate = now
            },
            new Project
            {
                ProjectId = 4, Name = "Downtown Arterial Resurfacing", Status = "Draft",
                StartDate = new DateOnly(2026, 9, 1), EndDate = null,
                TotalBudget = 750_000.00m, Location = "Main St & Oak Ave Corridor",
                CreatedDate = now, ModifiedDate = now
            },
            new Project
            {
                ProjectId = 5, Name = "Coastal Erosion Mitigation", Status = "Cancelled",
                StartDate = new DateOnly(2024, 4, 1), EndDate = new DateOnly(2025, 3, 31),
                TotalBudget = 8_900_000.00m, Location = "Pacific Coast Highway, MP 120-128",
                CreatedDate = now, ModifiedDate = now
            }
        );

        // --- Project Items (bid items) ---
        int itemId = 1;
        modelBuilder.Entity<ProjectItem>().HasData(
            // Project 1 items (Highway 101 Widening)
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "403-01", Description = "Hot Mix Asphalt Surface Course", Quantity = 15000m, Unit = "TON", UnitPrice = 85.50m, Amount = 1_282_500.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "403-02", Description = "Hot Mix Asphalt Base Course", Quantity = 25000m, Unit = "TON", UnitPrice = 72.00m, Amount = 1_800_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "501-01", Description = "Structural Concrete Class 4000", Quantity = 3500m, Unit = "CY", UnitPrice = 650.00m, Amount = 2_275_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "602-01", Description = "Steel Reinforcing Bar Grade 60", Quantity = 450000m, Unit = "LB", UnitPrice = 1.25m, Amount = 562_500.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "606-01", Description = "W-Beam Guardrail", Quantity = 12000m, Unit = "LF", UnitPrice = 32.00m, Amount = 384_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "611-01", Description = "36-inch Drainage Pipe", Quantity = 2400m, Unit = "LF", UnitPrice = 125.00m, Amount = 300_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "620-01", Description = "Traffic Signal Complete", Quantity = 4m, Unit = "EA", UnitPrice = 185_000.00m, Amount = 740_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "627-01", Description = "Pavement Marking Thermoplastic", Quantity = 85000m, Unit = "LF", UnitPrice = 2.50m, Amount = 212_500.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "110-01", Description = "Mobilization", Quantity = 1m, Unit = "LS", UnitPrice = 2_500_000.00m, Amount = 2_500_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "157-01", Description = "Erosion Control Blanket", Quantity = 45m, Unit = "AC", UnitPrice = 3_200.00m, Amount = 144_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "201-01", Description = "Clearing and Grubbing", Quantity = 25m, Unit = "AC", UnitPrice = 8_500.00m, Amount = 212_500.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 1, ItemNumber = "203-01", Description = "Excavation Unclassified", Quantity = 50000m, Unit = "CY", UnitPrice = 12.00m, Amount = 600_000.00m, CreatedDate = now },

            // Project 2 items (Bridge Rehab)
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "502-01", Description = "Bridge Deck Repair", Quantity = 8500m, Unit = "SF", UnitPrice = 185.00m, Amount = 1_572_500.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "502-02", Description = "Bridge Deck Overlay Polymer", Quantity = 8500m, Unit = "SF", UnitPrice = 45.00m, Amount = 382_500.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "504-01", Description = "Structural Steel Painting", Quantity = 15000m, Unit = "SF", UnitPrice = 28.00m, Amount = 420_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "506-01", Description = "Bearing Pad Replacement", Quantity = 12m, Unit = "EA", UnitPrice = 18_500.00m, Amount = 222_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "507-01", Description = "Expansion Joint Replacement", Quantity = 280m, Unit = "LF", UnitPrice = 450.00m, Amount = 126_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "602-02", Description = "Steel Reinforcing Bar Epoxy Coated", Quantity = 85000m, Unit = "LB", UnitPrice = 1.85m, Amount = 157_250.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "110-01", Description = "Mobilization", Quantity = 1m, Unit = "LS", UnitPrice = 625_000.00m, Amount = 625_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 2, ItemNumber = "614-01", Description = "Temporary Traffic Control", Quantity = 1m, Unit = "LS", UnitPrice = 480_000.00m, Amount = 480_000.00m, CreatedDate = now },

            // Project 3 items (Rural Safety)
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 3, ItemNumber = "606-02", Description = "Cable Median Barrier", Quantity = 8500m, Unit = "LF", UnitPrice = 42.00m, Amount = 357_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 3, ItemNumber = "620-02", Description = "Flashing Beacon Assembly", Quantity = 6m, Unit = "EA", UnitPrice = 25_000.00m, Amount = 150_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 3, ItemNumber = "625-01", Description = "Rumble Strip Milled", Quantity = 42000m, Unit = "LF", UnitPrice = 1.50m, Amount = 63_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 3, ItemNumber = "627-02", Description = "Raised Pavement Marker", Quantity = 500m, Unit = "EA", UnitPrice = 12.00m, Amount = 6_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 3, ItemNumber = "630-01", Description = "Sign Panel Type A", Quantity = 35m, Unit = "EA", UnitPrice = 850.00m, Amount = 29_750.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 3, ItemNumber = "110-01", Description = "Mobilization", Quantity = 1m, Unit = "LS", UnitPrice = 160_000.00m, Amount = 160_000.00m, CreatedDate = now },

            // Project 4 items (Resurfacing)
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 4, ItemNumber = "403-03", Description = "Hot Mix Asphalt Leveling Course", Quantity = 800m, Unit = "TON", UnitPrice = 95.00m, Amount = 76_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 4, ItemNumber = "403-01", Description = "Hot Mix Asphalt Surface Course", Quantity = 2200m, Unit = "TON", UnitPrice = 88.00m, Amount = 193_600.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 4, ItemNumber = "405-01", Description = "Tack Coat Emulsified Asphalt", Quantity = 5500m, Unit = "GAL", UnitPrice = 4.50m, Amount = 24_750.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 4, ItemNumber = "627-01", Description = "Pavement Marking Thermoplastic", Quantity = 18000m, Unit = "LF", UnitPrice = 2.50m, Amount = 45_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 4, ItemNumber = "110-01", Description = "Mobilization", Quantity = 1m, Unit = "LS", UnitPrice = 45_000.00m, Amount = 45_000.00m, CreatedDate = now },

            // Project 5 items (Coastal Erosion)
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 5, ItemNumber = "701-01", Description = "Riprap Class 2000lb", Quantity = 12000m, Unit = "TON", UnitPrice = 65.00m, Amount = 780_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 5, ItemNumber = "702-01", Description = "Geotextile Fabric", Quantity = 45000m, Unit = "SY", UnitPrice = 4.25m, Amount = 191_250.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 5, ItemNumber = "703-01", Description = "Retaining Wall Concrete MSE", Quantity = 3200m, Unit = "SF", UnitPrice = 125.00m, Amount = 400_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 5, ItemNumber = "203-02", Description = "Excavation Rock", Quantity = 8000m, Unit = "CY", UnitPrice = 45.00m, Amount = 360_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 5, ItemNumber = "157-02", Description = "Hydroseeding", Quantity = 30m, Unit = "AC", UnitPrice = 2_800.00m, Amount = 84_000.00m, CreatedDate = now },
            new ProjectItem { ProjectItemId = itemId++, ProjectId = 5, ItemNumber = "110-01", Description = "Mobilization", Quantity = 1m, Unit = "LS", UnitPrice = 445_000.00m, Amount = 445_000.00m, CreatedDate = now }
        );

        // --- Funding Sources ---
        int fsId = 1;
        modelBuilder.Entity<FundingSource>().HasData(
            // Project 1
            new FundingSource { FundingSourceId = fsId++, ProjectId = 1, SourceName = "Federal Highway Administration (FHWA)", FundingIdentifier = "FHWA-2025-OR-0042", AllocatedAmount = 36_000_000.00m, CreatedDate = now },
            new FundingSource { FundingSourceId = fsId++, ProjectId = 1, SourceName = "State Transportation Fund", FundingIdentifier = "STF-2025-118", AllocatedAmount = 9_000_000.00m, CreatedDate = now },
            // Project 2
            new FundingSource { FundingSourceId = fsId++, ProjectId = 2, SourceName = "National Highway Performance Program (NHPP)", FundingIdentifier = "NHPP-2023-OR-0188", AllocatedAmount = 10_000_000.00m, CreatedDate = now },
            new FundingSource { FundingSourceId = fsId++, ProjectId = 2, SourceName = "State Transportation Fund", FundingIdentifier = "STF-2023-076", AllocatedAmount = 2_500_000.00m, CreatedDate = now },
            // Project 3
            new FundingSource { FundingSourceId = fsId++, ProjectId = 3, SourceName = "Highway Safety Improvement Program (HSIP)", FundingIdentifier = "HSIP-2026-OR-0015", AllocatedAmount = 2_560_000.00m, CreatedDate = now },
            new FundingSource { FundingSourceId = fsId++, ProjectId = 3, SourceName = "Local Match - County Road Fund", FundingIdentifier = null, AllocatedAmount = 640_000.00m, CreatedDate = now },
            // Project 4
            new FundingSource { FundingSourceId = fsId++, ProjectId = 4, SourceName = "Surface Transportation Block Grant (STBG)", FundingIdentifier = "STBG-2026-OR-0301", AllocatedAmount = 600_000.00m, CreatedDate = now },
            new FundingSource { FundingSourceId = fsId++, ProjectId = 4, SourceName = "City Transportation Fund", FundingIdentifier = null, AllocatedAmount = 150_000.00m, CreatedDate = now },
            // Project 5
            new FundingSource { FundingSourceId = fsId++, ProjectId = 5, SourceName = "Federal Highway Administration (FHWA)", FundingIdentifier = "FHWA-2024-OR-0099", AllocatedAmount = 7_120_000.00m, CreatedDate = now },
            new FundingSource { FundingSourceId = fsId++, ProjectId = 5, SourceName = "State Emergency Fund", FundingIdentifier = "SEF-2024-031", AllocatedAmount = 1_780_000.00m, CreatedDate = now },
            // Additional sources
            new FundingSource { FundingSourceId = fsId++, ProjectId = 1, SourceName = "Federal Lands Access Program", FundingIdentifier = "FLAP-2025-OR-007", AllocatedAmount = 0.00m, CreatedDate = now },
            new FundingSource { FundingSourceId = fsId++, ProjectId = 2, SourceName = "Local Match - City of Portland", FundingIdentifier = null, AllocatedAmount = 0.00m, CreatedDate = now }
        );

        // --- Validation Issues ---
        int viId = 1;
        var detectDate = new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<ValidationIssue>().HasData(
            // Project 1 issues
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 1, Severity = "Warning", Description = "Project end date is more than 2 years from start date. Review timeline.", AffectedField = "EndDate", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 1, Severity = "Info", Description = "Federal funding requires annual progress report submission by March 31.", AffectedField = null, DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 1, Severity = "Warning", Description = "Mobilization cost exceeds 5% of total bid. Review for reasonableness.", AffectedField = "ProjectItems", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 1, Severity = "Info", Description = "Environmental clearance document expires 2027-06-30. Ensure project completion before expiration.", AffectedField = "EndDate", DetectedDate = detectDate },

            // Project 2 issues
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 2, Severity = "Info", Description = "Project marked as Completed. Final inspection report pending upload.", AffectedField = "Status", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 2, Severity = "Warning", Description = "Final cost exceeds original estimate by 8.2%. Requires justification memo.", AffectedField = "TotalBudget", DetectedDate = detectDate },

            // Project 3 issues
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 3, Severity = "Error", Description = "Project status is OnHold but no hold reason has been documented.", AffectedField = "Status", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 3, Severity = "Error", Description = "HSIP funding expires 2026-12-31. Current timeline extends beyond funding availability.", AffectedField = "EndDate", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 3, Severity = "Warning", Description = "Total bid items sum ($765,750) does not match project budget ($3,200,000). Missing items likely.", AffectedField = "TotalBudget", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 3, Severity = "Info", Description = "Safety study reference document should be attached.", AffectedField = null, DetectedDate = detectDate },

            // Project 4 issues
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 4, Severity = "Error", Description = "Project end date is not set. Required for scheduling and funding.", AffectedField = "EndDate", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 4, Severity = "Error", Description = "STBG funding agreement not yet executed. Cannot proceed without signed agreement.", AffectedField = "FundingSources", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 4, Severity = "Warning", Description = "Project is in Draft status. Bid items may be preliminary estimates.", AffectedField = "Status", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 4, Severity = "Info", Description = "City council approval required before advancing to Active status.", AffectedField = "Status", DetectedDate = detectDate },

            // Project 5 issues
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 5, Severity = "Error", Description = "Project is Cancelled but has unexpended federal funds. Requires fund de-obligation.", AffectedField = "FundingSources", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 5, Severity = "Warning", Description = "Cancellation reason not documented in project record.", AffectedField = "Status", DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 5, Severity = "Error", Description = "Environmental permits still active. Must be formally closed or transferred.", AffectedField = null, DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 5, Severity = "Info", Description = "Contractor final payment still outstanding ($23,450). Process closeout.", AffectedField = "TotalBudget", DetectedDate = detectDate },

            // Cross-cutting issues
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 1, Severity = "Error", Description = "DBE participation goal not set. Required for federal-aid projects.", AffectedField = null, DetectedDate = detectDate },
            new ValidationIssue { ValidationIssueId = viId++, ProjectId = 2, Severity = "Info", Description = "Project closeout checklist 80% complete. 2 items remaining.", AffectedField = null, DetectedDate = detectDate }
        );
    }
}
