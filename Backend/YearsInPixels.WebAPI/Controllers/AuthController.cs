using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YearsInPixels.Application.DTOs;
using YearsInPixels.Application.Interfaces;

namespace YearsInPixels.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ITokenService tokenService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok(new { message = "Token is valid", timestamp = DateTime.UtcNow });
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto?>> Register(UserDto request)
        {
            var result = await authService.RegisterAsync(request);
            if (result is null)
                return BadRequest("Username already exists.");

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await tokenService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }
    }
}
