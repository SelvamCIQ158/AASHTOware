using AASHTOware.ProjectDashboard.Api.Data;
using AASHTOware.ProjectDashboard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace AASHTOware.ProjectDashboard.Api.Controllers;

/// <summary>
/// OData controller for querying <see cref="Project"/> entities.
/// Supports standard OData query options including $filter, $select, $expand, $orderby, $top, and $skip.
/// </summary>
/// <example>
/// GET /odata/Projects?$filter=Status eq 'Active'
/// GET /odata/Projects?$select=ProjectId,Name,Status&amp;$orderby=Name
/// GET /odata/Projects(118456)
/// </example>
public class ProjectsController(ProjectDashboardDbContext db) : ODataController
{
    /// <summary>
    /// Returns a queryable collection of all projects.
    /// </summary>
    /// <returns>An <see cref="IQueryable{Project}"/> that can be further filtered using OData query options.</returns>
    [EnableQuery(MaxTop = 1000, PageSize = 100)]
    public IQueryable<Project> Get()
    {
        return db.Projects;
    }

    /// <summary>
    /// Returns a single project by its key.
    /// </summary>
    /// <param name="key">The unique identifier of the project.</param>
    /// <returns>A <see cref="SingleResult{Project}"/> containing the matching project, if found.</returns>
    [EnableQuery(MaxTop = 1000, PageSize = 100)]
    public SingleResult<Project> Get(int key)
    {
        return SingleResult.Create(db.Projects.Where(p => p.ProjectId == key));
    }
}
