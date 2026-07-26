using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TruckLink.API.DTOs;
using TruckLink.Core.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TruckLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _authService.RegisterAsync(dto.Name, dto.Email, dto.Password);
        if (!success)
            return Conflict(ApiResponse<object>.Error("Email already in use", (int) HttpStatusCode.Conflict));

        return Ok(ApiResponse<object>.Success(null,"Registration successful", (int) HttpStatusCode.OK));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Error("Payload is not correct",(int) HttpStatusCode.BadRequest));

        var result = await _authService.LoginAsync(dto.Email, dto.Password);
        if (result == null)
            return Unauthorized(ApiResponse<object>.Error("Credentials are wrong", (int) HttpStatusCode.Unauthorized));

        Response.Cookies.Append("refreshToken", result.Value.refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        return Ok(ApiResponse<object>.Success(new { token = result.Value.accessToken }, "Login successful", (int)HttpStatusCode.OK));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var result = await _authService.refresh(refreshToken);

        if (result == null)
        {
            return Unauthorized(ApiResponse<object>.Error("Credentials are wrong", (int)HttpStatusCode.Unauthorized));
        }

        return Ok(new { accessToken = result });
    }

    [Authorize]
    [HttpGet("roles")]
    public async Task<IActionResult> GetMyRoles()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var roles = await _authService.GetUserRolesAsync(userId);

        return Ok(roles);
    }
}
