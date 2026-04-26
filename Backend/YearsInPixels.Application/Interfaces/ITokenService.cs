using YearsInPixels.Application.DTOs;
using YearsInPixels.Domain.Entities;

namespace YearsInPixels.Application.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<TokenResponseDto> CreateTokenResponse(User user);
    }
}
