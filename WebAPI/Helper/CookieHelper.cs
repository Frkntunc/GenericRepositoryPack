using Microsoft.AspNetCore.Http;

namespace WebAPI.Helper
{
    public static class CookieHelper
    {
        public static void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken)
        {
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
            response.Cookies.Append("refreshToken", refreshToken, refreshTokenOptions);
        }

    }
}
