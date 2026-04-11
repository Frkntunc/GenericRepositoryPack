namespace Shared.Options
{
    public class CookieTokenOptions
    {
        public string AccessTokenCookieName { get; set; } = "accessToken";
        public string RefreshTokenCookieName { get; set; } = "refreshToken";
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
