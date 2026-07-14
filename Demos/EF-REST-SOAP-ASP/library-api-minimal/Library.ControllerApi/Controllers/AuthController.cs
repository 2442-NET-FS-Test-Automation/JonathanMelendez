using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using Library.ControllerApi.Services;
using Library.ControllerApi.DTOs;
using Library.Data.Entities;

[ApiController]
[Route("auth")]
public class AuthController(ITokenService tokens, IUserService users) : ControllerBase
{
    private readonly ITokenService _tokens = tokens;
    private readonly IUserService _users = users;

    [HttpPost("register")]
    public async Task<ActionResult> RegisterConsumer(RegisterDto dto)
    {
        var error = await _users.RegisterAsync(dto.UserName, dto.Password);

        if (error is not null) return Conflict(new { error });
        return CreatedAtAction(nameof(Me), null);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var user = await _users.ValidateAsync(dto.UserName, dto.Password);
        if(user is null) return Unauthorized(new { error = "bad credentials"});
        return Ok(new
        {
            token = _tokens.Issue(user.UserName, user.Role)
        });
    }

    [HttpGet("me")]
    public ActionResult Me()
    {
        return Ok(new
        {
            name = User.Identity?.Name,
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [HttpPost("token")]
    public ActionResult GetToken(string username, UserRoles role)
    {
        return Ok(_tokens.Issue(username, role));
    }
}