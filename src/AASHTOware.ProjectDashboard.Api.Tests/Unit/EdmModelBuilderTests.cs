namespace AASHTOware.ProjectDashboard.Api.Tests.Unit;

using AASHTOware.ProjectDashboard.Api.OData;
using FluentAssertions;
using Microsoft.OData.Edm;

public class EdmModelBuilderTests
{
    private readonly IEdmModel _model = EdmModelBuilder.GetEdmModel();

    [Fact]
    public void GetEdmModel_ContainsProjectsEntitySet()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("Projects");
        entitySet.Should().NotBeNull();
    }

    [Fact]
    public void GetEdmModel_ContainsProjectItemsEntitySet()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("ProjectItems");
        entitySet.Should().NotBeNull();
    }

    [Fact]
    public void GetEdmModel_ContainsFundingSourcesEntitySet()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("FundingSources");
        entitySet.Should().NotBeNull();
    }

    [Fact]
    public void GetEdmModel_ContainsValidationIssuesEntitySet()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("ValidationIssues");
        entitySet.Should().NotBeNull();
    }

    [Fact]
    public void GetEdmModel_ProjectEntityKeyIsProjectId()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("Projects");
        var entityType = entitySet!.EntityType;
        var keys = entityType.Key().ToList();

        keys.Should().HaveCount(1);
        keys[0].Name.Should().Be("ProjectId");
    }

    [Fact]
    public void GetEdmModel_ProjectItemEntityKeyIsProjectItemId()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("ProjectItems");
        var entityType = entitySet!.EntityType;
        var keys = entityType.Key().ToList();

        keys.Should().HaveCount(1);
        keys[0].Name.Should().Be("ProjectItemId");
    }

    [Fact]
    public void GetEdmModel_FundingSourceEntityKeyIsFundingSourceId()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("FundingSources");
        var entityType = entitySet!.EntityType;
        var keys = entityType.Key().ToList();

        keys.Should().HaveCount(1);
        keys[0].Name.Should().Be("FundingSourceId");
    }

    [Fact]
    public void GetEdmModel_ValidationIssueEntityKeyIsValidationIssueId()
    {
        var entitySet = _model.EntityContainer.FindEntitySet("ValidationIssues");
        var entityType = entitySet!.EntityType;
        var keys = entityType.Key().ToList();

        keys.Should().HaveCount(1);
        keys[0].Name.Should().Be("ValidationIssueId");
    }
}
