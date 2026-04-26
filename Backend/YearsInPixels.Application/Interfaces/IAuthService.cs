using YearsInPixels.Application.DTOs;

namespace YearsInPixels.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
