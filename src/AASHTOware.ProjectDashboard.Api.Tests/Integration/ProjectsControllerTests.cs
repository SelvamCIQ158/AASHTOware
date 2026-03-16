namespace AASHTOware.ProjectDashboard.Api.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

public class ProjectsControllerTests : IClassFixture<ProjectDashboardWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;

    public ProjectsControllerTests(ProjectDashboardWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ById_ReturnsOkWithProject()
    {
        // Act
        var response = await _client.GetAsync("/odata/Projects(1)");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        json.GetProperty("ProjectId").GetInt32().Should().Be(1);
        json.GetProperty("Name").GetString().Should().Be("Highway 101 Widening Phase 2");
        json.GetProperty("Status").GetString().Should().Be("Active");
    }

    [Fact]
    public async Task Get_ById_NonExistent_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/odata/Projects(9999)");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_WithSelect_ReturnsOnlySelectedProperties()
    {
        // Act
        var response = await _client.GetAsync("/odata/Projects?$select=ProjectId,Name");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().BeGreaterThan(0);

        var first = items[0];
        first.TryGetProperty("ProjectId", out _).Should().BeTrue();
        first.TryGetProperty("Name", out _).Should().BeTrue();
        // Status should not be present when not selected
        first.TryGetProperty("Status", out _).Should().BeFalse();
        first.TryGetProperty("TotalBudget", out _).Should().BeFalse();
    }

    [Fact]
    public async Task Get_WithExpandProjectItems_ReturnsNestedItems()
    {
        // Act
        var response = await _client.GetAsync("/odata/Projects(1)?$expand=ProjectItems");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        json.GetProperty("ProjectId").GetInt32().Should().Be(1);

        var items = json.GetProperty("ProjectItems");
        items.GetArrayLength().Should().Be(3);

        // Verify the nested items belong to project 1
        foreach (var item in items.EnumerateArray())
        {
            item.GetProperty("ProjectId").GetInt32().Should().Be(1);
        }
    }

    [Fact]
    public async Task Get_WithFilterByStatus_ReturnsFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/odata/Projects?$filter=Status eq 'Active'");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().BeGreaterThan(0);

        foreach (var item in items.EnumerateArray())
        {
            item.GetProperty("Status").GetString().Should().Be("Active");
        }
    }

    [Fact]
    public async Task Get_WithCount_ReturnsCountInResponse()
    {
        // Act
        var response = await _client.GetAsync("/odata/Projects?$count=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        json.TryGetProperty("@odata.count", out var count).Should().BeTrue();
        count.GetInt32().Should().Be(3);
    }
}
