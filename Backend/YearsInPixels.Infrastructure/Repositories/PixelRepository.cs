using Microsoft.EntityFrameworkCore;
using YearsInPixels.Domain.Entities;
using YearsInPixels.Domain.Interfaces;

namespace YearsInPixels.Infrastructure.Repositories
{
    public class PixelRepository(AppDbContext context) : IPixelRepository
    {
        public async Task<IEnumerable<DailyPixel>> GetPixelsForYearAsync(Guid userId, int year)
        {
            return await context.DailyPixels
                .Where(p => p.UserId == userId && p.Date.Year == year)
                .ToListAsync();
        }

        public async Task<DailyPixel?> GetPixelByDateAsync(Guid userId, DateTime date)
        {
            var dateOnly = date.Date;
            return await context.DailyPixels
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Date == dateOnly);
        }

        public async Task<DailyPixel?> GetRandomPixelAsync(Guid userId, int? excludedId)
        {
            var query = context.DailyPixels.Where(p => p.UserId == userId);

            var totalCount = await query.CountAsync();

            if (excludedId.HasValue && totalCount > 1)
            {
                query = query.Where(p => p.Id != excludedId);
            }

            return await query
                .OrderBy(r => EF.Functions.Random())
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(DailyPixel pixel) => await context.DailyPixels.AddAsync(pixel);

        public async Task SaveChangesAsync() => await context.SaveChangesAsync();
    }
}
