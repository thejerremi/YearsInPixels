using FluentValidation;
using Microsoft.AspNetCore.Identity;
using YearsInPixels.Application.DTOs;
using YearsInPixels.Application.Interfaces;
using YearsInPixels.Domain.Entities;
using YearsInPixels.Domain.Interfaces;

namespace YearsInPixels.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserDto> _validator;
        private readonly ITokenService _tokenService;
        public AuthService(IUserRepository userRepository, IValidator<UserDto> validator, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _validator = validator;
            _tokenService = tokenService;
        }
        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await _tokenService.CreateTokenResponse(user);
        }



        public async Task<TokenResponseDto?> RegisterAsync(UserDto request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (await _userRepository.AnyUserAsync(request.Username))
            {
                return null;
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return await _tokenService.CreateTokenResponse(user);
        }
    }
}
