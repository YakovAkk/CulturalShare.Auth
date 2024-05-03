using AuthenticationProto;
using CulturalShare.Auth.Services.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CulturalShare.Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _log;
    public AuthController(IAuthService authService, ILogger<AuthController> log)
    {
        _authService = authService;
        _log = log;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserControllerActionAsync([FromBody] RegistrationRequest request)
    {
        var userId = await _authService.CreateUserAsync(request);
        return Ok(userId);
    }
}
