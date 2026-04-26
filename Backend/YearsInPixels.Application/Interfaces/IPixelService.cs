using YearsInPixels.Application.DTOs;

namespace YearsInPixels.Application.Interfaces
{
    public interface IPixelService
    {
        Task<IEnumerable<DailyPixelDto>> GetYearlyPixelsAsync(int year);
        Task AddOrUpdatePixelAsync(DailyPixelDto dto);
        Task<RandomPixelDto?> GetRandomFlashbackAsync(int? id);
    }
}
