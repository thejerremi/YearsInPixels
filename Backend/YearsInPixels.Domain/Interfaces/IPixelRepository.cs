using YearsInPixels.Domain.Entities;

namespace YearsInPixels.Domain.Interfaces
{
    public interface IPixelRepository
    {
        Task<IEnumerable<DailyPixel>> GetPixelsForYearAsync(Guid userId, int year);
        Task<DailyPixel?> GetPixelByDateAsync(Guid userId, DateTime date);
        Task<DailyPixel?> GetRandomPixelAsync(Guid userId, int? excludedId);
        Task AddAsync(DailyPixel pixel);
        Task SaveChangesAsync();
    }
}
