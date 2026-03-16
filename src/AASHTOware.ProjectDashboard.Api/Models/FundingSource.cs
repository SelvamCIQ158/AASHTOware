namespace AASHTOware.ProjectDashboard.Api.Models;

public class FundingSource
{
    public int FundingSourceId { get; set; }
    public int ProjectId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string? FundingIdentifier { get; set; }
    public decimal AllocatedAmount { get; set; }
    public DateTime CreatedDate { get; set; }

    public Project Project { get; set; } = null!;
}
