using System.ComponentModel.DataAnnotations;

namespace YearsInPixels.Domain.Entities
{
    public class DailyPixel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Range(0, 100)]
        public int MoodValue { get; set; }
        [MaxLength(30)]
        public string DayWord { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string JournalNote { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
