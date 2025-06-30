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
            return BadRequest(new ApiResponse<object>
            {
                IsSuccess = false,
                Message = "Email already in use.",
                Code = 400
            });

        return Ok(new ApiResponse<object>
        {
            IsSuccess = true,
            Message = "Registration successful",
            Code = 200
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<object>
            {
                IsSuccess = false,
                Message = "Payload is not correct",
                Code = 500
            });

        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token == null)
            return Unauthorized(new ApiResponse<object>
            {
                IsSuccess = false,
                Message = "Credentials are wrong",
                Code = 500
            });

        return Ok(new ApiResponse<object>
        {
            IsSuccess = true,
            Message = "Login successful",
            Code = 200,
            Data = new { token = token }
        });
    }
}
