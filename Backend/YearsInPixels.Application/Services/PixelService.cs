using FluentValidation;
using YearsInPixels.Application.DTOs;
using YearsInPixels.Application.Interfaces;
using YearsInPixels.Domain.Entities;
using YearsInPixels.Domain.Interfaces;

namespace YearsInPixels.Application.Services
{

    public class PixelService : IPixelService
    {
        private readonly IPixelRepository _pixelRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IValidator<DailyPixelDto> _validator;
        public PixelService(IPixelRepository pixelRepository, ICurrentUserService currentUser, IValidator<DailyPixelDto> validator)
        {
            _pixelRepository = pixelRepository;
            _currentUser = currentUser;
            _validator = validator;
        }

        public async Task<IEnumerable<DailyPixelDto>> GetYearlyPixelsAsync(int year)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
            var pixels = await _pixelRepository.GetPixelsForYearAsync(userId, year);

            return pixels.Select(p => new DailyPixelDto(p.Date, p.MoodValue, p.DayWord, p.JournalNote));
        }

        public async Task AddOrUpdatePixelAsync(DailyPixelDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var existingPixel = await _pixelRepository.GetPixelByDateAsync(userId, dto.Date);

            if (existingPixel != null)
            {
                existingPixel.MoodValue = dto.MoodValue;
                existingPixel.DayWord = dto.DayWord;
                existingPixel.JournalNote = dto.JournalNote;
            }
            else
            {
                var newPixel = new DailyPixel
                {
                    UserId = userId,
                    Date = dto.Date.Date,
                    MoodValue = dto.MoodValue,
                    DayWord = dto.DayWord,
                    JournalNote = dto.JournalNote
                };
                await _pixelRepository.AddAsync(newPixel);
            }

            await _pixelRepository.SaveChangesAsync();
        }

        public async Task<RandomPixelDto?> GetRandomFlashbackAsync(int? id)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
            var pixel = await _pixelRepository.GetRandomPixelAsync(userId, id);

            if (pixel == null) return null;

            return new RandomPixelDto(pixel.Id, pixel.Date, pixel.MoodValue, pixel.DayWord, pixel.JournalNote);
        }
    }
}
