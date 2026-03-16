namespace AASHTOware.ProjectDashboard.Api.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

public class ProjectItemsControllerTests : IClassFixture<ProjectDashboardWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;

    public ProjectItemsControllerTests(ProjectDashboardWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_WithFilterByProjectId_ReturnsItemsForProject()
    {
        // Act
        var response = await _client.GetAsync("/odata/ProjectItems?$filter=ProjectId eq 1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().Be(3);

        foreach (var item in items.EnumerateArray())
        {
            item.GetProperty("ProjectId").GetInt32().Should().Be(1);
        }
    }

    [Fact]
    public async Task Get_WithOrderByAmountDesc_ReturnsDescendingOrder()
    {
        // Act
        var response = await _client.GetAsync("/odata/ProjectItems?$filter=ProjectId eq 1&$orderby=Amount desc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");

        var amounts = new List<decimal>();
        foreach (var item in items.EnumerateArray())
        {
            amounts.Add(item.GetProperty("Amount").GetDecimal());
        }

        amounts.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Get_WithTop_LimitsResults()
    {
        // We have 5 total items seeded across projects; request top 2
        // Act
        var response = await _client.GetAsync("/odata/ProjectItems?$top=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task Get_WithCountTrue_ReturnsItemCount()
    {
        // Act
        var response = await _client.GetAsync("/odata/ProjectItems?$count=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        json.TryGetProperty("@odata.count", out var count).Should().BeTrue();
        count.GetInt32().Should().Be(5);
    }

    [Fact]
    public async Task Get_FilterByNonExistentProject_ReturnsEmptyCollection()
    {
        // Act
        var response = await _client.GetAsync("/odata/ProjectItems?$filter=ProjectId eq 9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().Be(0);
    }
}
