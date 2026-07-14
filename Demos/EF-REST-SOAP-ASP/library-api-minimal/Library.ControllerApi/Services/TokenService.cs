using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Library.Data.Entities;


namespace Library.ControllerApi.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    private readonly string _key = config["Jwt:Key"]!;
    public string Issue(string user, UserRoles role)
    {
        // Sign the token with a symetric key
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            SecurityAlgorithms.HmacSha256
        );
        // Then register claims
        var token = new JwtSecurityToken(
            "library-fulfillment", 
            "library-fulfillment-clients", 
            [new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Role, role.ToString())]
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}