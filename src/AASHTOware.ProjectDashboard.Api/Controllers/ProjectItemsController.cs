using AASHTOware.ProjectDashboard.Api.Data;
using AASHTOware.ProjectDashboard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace AASHTOware.ProjectDashboard.Api.Controllers;

/// <summary>
/// OData controller for querying <see cref="ProjectItem"/> entities.
/// Supports standard OData query options including $filter, $select, $expand, $orderby, $top, and $skip.
/// </summary>
/// <example>
/// GET /odata/ProjectItems?$filter=ProjectId eq 118456&amp;$orderby=Amount desc&amp;$top=10
/// GET /odata/ProjectItems?$select=ProjectItemId,Description,Amount
/// </example>
public class ProjectItemsController(ProjectDashboardDbContext db) : ODataController
{
    /// <summary>
    /// Returns a queryable collection of all project items.
    /// </summary>
    /// <returns>An <see cref="IQueryable{ProjectItem}"/> that can be further filtered using OData query options.</returns>
    [EnableQuery(MaxTop = 1000, PageSize = 100)]
    public IQueryable<ProjectItem> Get()
    {
        return db.ProjectItems;
    }
}
