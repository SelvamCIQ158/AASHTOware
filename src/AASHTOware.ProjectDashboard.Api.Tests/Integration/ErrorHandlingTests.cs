namespace AASHTOware.ProjectDashboard.Api.Tests.Integration;

using System.Net;
using FluentAssertions;

public class ErrorHandlingTests : IClassFixture<ProjectDashboardWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ErrorHandlingTests(ProjectDashboardWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_WithInvalidFilterField_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/odata/Projects?$filter=NonExistentField eq 'value'");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
