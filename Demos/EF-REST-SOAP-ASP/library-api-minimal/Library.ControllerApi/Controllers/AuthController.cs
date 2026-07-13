using Microsoft.AspNetCore.Mvc;

using Library.ControllerApi.Services;

[ApiController]
[Route("auth")]
public class AuthController(ITokenService tokens) : ControllerBase
{
    private readonly ITokenService _tokens = tokens;

    [HttpPost("token")]
    public ActionResult GetToken(string username)
    {
        return Ok(_tokens.Issue(username));
    }
}