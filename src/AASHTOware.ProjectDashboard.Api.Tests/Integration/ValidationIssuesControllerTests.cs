namespace AASHTOware.ProjectDashboard.Api.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

public class ValidationIssuesControllerTests : IClassFixture<ProjectDashboardWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;

    public ValidationIssuesControllerTests(ProjectDashboardWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_WithFilterByProjectId_ReturnsIssuesForProject()
    {
        // Act
        var response = await _client.GetAsync("/odata/ValidationIssues?$filter=ProjectId eq 1");

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
    public async Task Get_WithOrderBySeverity_ReturnsSortedResults()
    {
        // Act
        var response = await _client.GetAsync("/odata/ValidationIssues?$orderby=Severity");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");

        var severities = new List<string>();
        foreach (var item in items.EnumerateArray())
        {
            severities.Add(item.GetProperty("Severity").GetString()!);
        }

        severities.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Get_WithFilterBySeverityError_ReturnsOnlyErrors()
    {
        // Act
        var response = await _client.GetAsync("/odata/ValidationIssues?$filter=Severity eq 'Error'");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().BeGreaterThan(0);

        foreach (var item in items.EnumerateArray())
        {
            item.GetProperty("Severity").GetString().Should().Be("Error");
        }
    }

    [Fact]
    public async Task Get_FilterByNonExistentProject_ReturnsEmptyCollection()
    {
        // Act
        var response = await _client.GetAsync("/odata/ValidationIssues?$filter=ProjectId eq 9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var items = json.GetProperty("value");
        items.GetArrayLength().Should().Be(0);
    }
}
