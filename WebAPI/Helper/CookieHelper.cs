using Microsoft.AspNetCore.Http;

namespace WebAPI.Helper
{
    public static class CookieHelper
    {
        public static void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken, string csrfToken)
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

            var csrfTokenOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,     
                SameSite = SameSiteMode.Strict,
                Path = "/"
            };

            response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
            response.Cookies.Append("refreshToken", refreshToken, refreshTokenOptions);
            response.Cookies.Append("XSRF-TOKEN", refreshToken, csrfTokenOptions);
        }

    }
}
