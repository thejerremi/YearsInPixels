using Microsoft.EntityFrameworkCore;
using YearsInPixels.Domain.Entities;

namespace YearsInPixels.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<DailyPixel> DailyPixels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyPixel>()
                .HasOne(p => p.User)
                .WithMany(u => u.Pixels)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DailyPixel>()
                .HasIndex(p => new { p.UserId, p.Date })
                .IsUnique();
        }
    }

}
