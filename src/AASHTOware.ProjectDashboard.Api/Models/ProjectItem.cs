namespace AASHTOware.ProjectDashboard.Api.Models;

public class ProjectItem
{
    public int ProjectItemId { get; set; }
    public int ProjectId { get; set; }
    public string ItemNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDate { get; set; }

    public Project Project { get; set; } = null!;
}
