using AASHTOware.ProjectDashboard.Api.Data;
using AASHTOware.ProjectDashboard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace AASHTOware.ProjectDashboard.Api.Controllers;

/// <summary>
/// OData controller for querying <see cref="ValidationIssue"/> entities.
/// Supports standard OData query options including $filter, $select, $expand, $orderby, $top, and $skip.
/// </summary>
/// <example>
/// GET /odata/ValidationIssues?$filter=ProjectId eq 118456
/// GET /odata/ValidationIssues?$select=ValidationIssueId,Severity,Message&amp;$orderby=Severity
/// </example>
public class ValidationIssuesController(ProjectDashboardDbContext db) : ODataController
{
    /// <summary>
    /// Returns a queryable collection of all validation issues.
    /// </summary>
    /// <returns>An <see cref="IQueryable{ValidationIssue}"/> that can be further filtered using OData query options.</returns>
    [EnableQuery(MaxTop = 1000, PageSize = 100)]
    public IQueryable<ValidationIssue> Get()
    {
        return db.ValidationIssues;
    }
}
