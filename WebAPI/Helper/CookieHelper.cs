using Microsoft.AspNetCore.Http;
using Shared.Options;

namespace WebAPI.Helper
{
    public static class CookieHelper
    {
        public static void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken, CookieTokenOptions cookieTokenOptions, CsrfOptions csrfOptions, string? csrfToken = null)
        {
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(cookieTokenOptions.AccessTokenExpirationMinutes)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(cookieTokenOptions.RefreshTokenExpirationDays)
            };

            var csrfTokenOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(cookieTokenOptions.AccessTokenExpirationMinutes)
            };

            response.Cookies.Append(cookieTokenOptions.AccessTokenCookieName, accessToken, accessTokenOptions);
            response.Cookies.Append(cookieTokenOptions.RefreshTokenCookieName, refreshToken, refreshTokenOptions);

            if (!string.IsNullOrEmpty(csrfToken))
            {
                response.Cookies.Append(csrfOptions.CookieName, csrfToken, csrfTokenOptions);
            }
        }
    }
}
