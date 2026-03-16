namespace AASHTOware.ProjectDashboard.Api.Models;

public class Project
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal TotalBudget { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public ICollection<ProjectItem> ProjectItems { get; set; } = [];
    public ICollection<FundingSource> FundingSources { get; set; } = [];
    public ICollection<ValidationIssue> ValidationIssues { get; set; } = [];
}
