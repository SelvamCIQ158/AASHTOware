using AASHTOware.ProjectDashboard.Api.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace AASHTOware.ProjectDashboard.Api.OData;

public static class EdmModelBuilder
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();

        builder.EntitySet<Project>("Projects");
        builder.EntitySet<ProjectItem>("ProjectItems");
        builder.EntitySet<FundingSource>("FundingSources");
        builder.EntitySet<ValidationIssue>("ValidationIssues");

        return builder.GetEdmModel();
    }
}
