namespace YearsInPixels.Application.Configurations
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; } = 10;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
