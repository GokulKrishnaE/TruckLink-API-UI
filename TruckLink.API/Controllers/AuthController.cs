using System.Net;
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

        var success = await _authService.RegisterAsync(dto.Name, dto.Email, dto.Password, dto.Role);
        if (!success)
            return Conflict(ApiResponse<object>.Error("Email already in use", (int) HttpStatusCode.Conflict));

        return Ok(ApiResponse<object>.Success(null,"Registration successful", (int) HttpStatusCode.OK));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Error("Payload is not correct",(int) HttpStatusCode.BadRequest));

        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token == null)
            return Unauthorized(ApiResponse<object>.Error("Credentials are wrong", (int) HttpStatusCode.Unauthorized));

        return Ok(ApiResponse<object>.Success(new { token = token }, "Login successful", (int)HttpStatusCode.OK));
    }
}
