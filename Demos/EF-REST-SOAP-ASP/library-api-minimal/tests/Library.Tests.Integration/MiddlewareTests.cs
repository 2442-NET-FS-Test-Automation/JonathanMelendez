using System.Net;
using FluentAssertions;

namespace Library.Tests.Integration;

[Collection("Library API")]
public class MiddlewareTests
{
    private readonly HttpClient _client;

    public MiddlewareTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task XMaintenanceHeader_ShortCircuitWith503()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Inventory");
        request.Headers.Add("X-Maintenance", "1");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

    }
}