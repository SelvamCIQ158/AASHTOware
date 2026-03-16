using AASHTOware.ProjectDashboard.Api.Data;
using AASHTOware.ProjectDashboard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace AASHTOware.ProjectDashboard.Api.Controllers;

/// <summary>
/// OData controller for querying <see cref="FundingSource"/> entities.
/// Supports standard OData query options including $filter, $select, $expand, $orderby, $top, and $skip.
/// </summary>
/// <example>
/// GET /odata/FundingSources?$filter=ProjectId eq 118456
/// GET /odata/FundingSources?$select=FundingSourceId,Name,Amount&amp;$orderby=Name
/// </example>
public class FundingSourcesController(ProjectDashboardDbContext db) : ODataController
{
    /// <summary>
    /// Returns a queryable collection of all funding sources.
    /// </summary>
    /// <returns>An <see cref="IQueryable{FundingSource}"/> that can be further filtered using OData query options.</returns>
    [EnableQuery(MaxTop = 1000, PageSize = 100)]
    public IQueryable<FundingSource> Get()
    {
        return db.FundingSources;
    }
}
