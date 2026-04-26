using FluentValidation;
using YearsInPixels.Application.DTOs;

namespace YearsInPixels.Application.Validators
{
    public class DailyPixelDtoValidator : AbstractValidator<DailyPixelDto>
    {
        public DailyPixelDtoValidator()
        {
            RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.MoodValue)
                .InclusiveBetween(0, 100)
                .WithMessage("Mood value must be between 0 and 100.");

            RuleFor(x => x.DayWord)
                .MaximumLength(30)
                .WithMessage("The day word is too long (max {MaxLength} characters).");

            RuleFor(x => x.JournalNote)
                .MaximumLength(2000)
                .WithMessage("Your note exceeds the maximum length of {MaxLength} characters.");

            RuleFor(x => x.Date.Date)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("You cannot record a pixel for a future date.");
        }
    }
}
