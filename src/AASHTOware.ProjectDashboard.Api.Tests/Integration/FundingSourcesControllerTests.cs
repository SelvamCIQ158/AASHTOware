namespace AASHTOware.ProjectDashboard.Api.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

public class FundingSourcesControllerTests : IClassFixture<ProjectDashboardWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;

    public FundingSourcesControllerTests(ProjectDashboardWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_WithFilterByProjectId_ReturnsFundingSourcesForProject()
    {
        // Act
        var response = await _client.GetAsync("/odata/FundingSources?$filter=ProjectId eq 1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().Be(2);

        foreach (var item in items.EnumerateArray())
        {
            item.GetProperty("ProjectId").GetInt32().Should().Be(1);
        }
    }

    [Fact]
    public async Task Get_FilterByNonExistentProject_ReturnsEmptyCollection()
    {
        // Act
        var response = await _client.GetAsync("/odata/FundingSources?$filter=ProjectId eq 9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task Get_FilterByProjectId_DoesNotReturnOtherProjectSources()
    {
        // Act
        var response = await _client.GetAsync("/odata/FundingSources?$filter=ProjectId eq 2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().Be(1);

        // Verify no items from project 1 leaked through
        foreach (var item in items.EnumerateArray())
        {
            item.GetProperty("ProjectId").GetInt32().Should().Be(2);
            item.GetProperty("SourceName").GetString().Should().Contain("NHPP");
        }
    }
}
