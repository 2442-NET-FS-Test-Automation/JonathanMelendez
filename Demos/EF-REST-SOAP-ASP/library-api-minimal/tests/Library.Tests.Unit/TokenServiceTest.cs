using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

using Library.ControllerApi.Services;
using Library.Data.Entities;
using Xunit.Abstractions;

namespace Library.Tests.Unit;

public class TokenServiceTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;
    private const string TestKey = "unit-test-key-ipouyitdrdyxcfvbhnjmpoṕiuytrfgbhjkplolikytrewdfgvbhjkl";
    
    private static TokenService CreateSUT()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
                {["Jwt:key"] = TestKey}).Build();
        
        return new TokenService(config);
    }

    [Fact]
    public void Issue_RetursParsableJwt()
    {
        // Arrange
        var sut = CreateSUT();
    
        // Act
        var token = sut.Issue("ada", UserRoles.Admin);
        _output.WriteLine(token);
        
        // Assert
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);
        parsed.Issuer.Should().Be("library-fulfillment");
        parsed.Audiences.Should().Contain("library-fulfillment-clients");

        Assert.Equal("library-fulfillment", parsed.Issuer);
        Assert.Contains("library-fulfillment-clients", parsed.Audiences);
    }

    [Fact]
    public void Issue_IncludesNameAndRoleClaims()
    {
        // Arrange
        var sut = CreateSUT();

        // Act
        var token = sut.Issue("ada", UserRoles.Admin);

        // Assert
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        parsed.Claims.Should().Contain(c => c.Value == "ada" &&
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

        parsed.Claims.Should().Contain(c => c.Value == "Admin" &&
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
    } 

    [Theory]
    [InlineData("ada", UserRoles.Admin)]
    [InlineData("grace", UserRoles.Consumer)]
    public void Issue_SetRoleClaim_ForAnyRole(string user, UserRoles role)
    {
        // Arrange
        var sut = CreateSUT();

        // Act
        var token = sut.Issue(user, role);

        // Assert
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        parsed.Claims.Should().Contain(c => c.Value == role.ToString() &&
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
    }
}