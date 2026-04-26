namespace YearsInPixels.Application.DTOs
{
    public record DailyPixelDto(
        DateTime Date,
        int MoodValue,
        string DayWord,
        string JournalNote
    );

    public record RandomPixelDto(
        int id,
        DateTime Date,
        int MoodValue,
        string DayWord,
        string JournalNote
    );
}
