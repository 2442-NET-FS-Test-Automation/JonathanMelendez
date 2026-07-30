using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Library.ControllerApi.DTOs;

namespace Library.Tests.Integration;

[Collection("Library API")]
public class ModelValidationTests
{
    private readonly HttpClient _client;

    public ModelValidationTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
    }
    private record TokenResponse(string token);

    [Fact]
    public void DirectorValidator_MissesPositionalRecordAttributes()
    {
        // Given
        var dto = new InventoryDTO("BK-BAD", "Bad Book", 1, -50m);
        var results = new List<ValidationResult>();

        // When
        var valid = Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        // Then
        valid.Should().Be(true);
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task PostInventory_WithInvlidBody_Returns400()
    {
        // Given
        var login = await _client.PostAsJsonAsync("/auth/login",
            new { username = "ada", password = "pass123!"});
        var token = (await login.Content.ReadFromJsonAsync<TokenResponse>())!.token;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new InventoryDTO("BK-BAD", "Bad Book", 1, -50m);
    
        // When
        var response = await _client.PostAsJsonAsync("/api/Inventory", dto);
    
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}