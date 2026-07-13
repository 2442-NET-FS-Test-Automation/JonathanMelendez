using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace Library.ControllerApi.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    private readonly string _key = config["Jwt:Key"]!;
    private static readonly Dictionary<string, string> Roles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ada"] = "admin"  
    };
    public string Issue(string user)
    {
        // Sign the token with a symetric key
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            SecurityAlgorithms.HmacSha256
        );
        var role = Roles.GetValueOrDefault(user, "consumer");
        // Then register claims
        var token = new JwtSecurityToken(
            "library-fulfillment", 
            "library-fulfillment-clients", 
            [new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Role, role)]
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}