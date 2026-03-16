namespace AASHTOware.ProjectDashboard.Api.Models;

public class ValidationIssue
{
    public int ValidationIssueId { get; set; }
    public int ProjectId { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AffectedField { get; set; }
    public DateTime DetectedDate { get; set; }

    public Project Project { get; set; } = null!;
}
