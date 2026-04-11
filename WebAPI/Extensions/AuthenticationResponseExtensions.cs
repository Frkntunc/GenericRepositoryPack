using ApplicationService.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.DTOs.Common;
using Shared.Options;
using WebAPI.Helper;

namespace WebAPI.Extensions
{
    public static class AuthenticationResponseExtensions
    {
        private const string ClientTypeHeader = "X-Client-Type";
        private const string WebClient = "Web";

        public static ServiceResponse<T> ReturnClientAwareAuthResult<T>(this ControllerBase controller, ServiceResponse<T> response)
        {
            if (response is null || response.Data is null)
            {
                return response;
            }

            var request = controller.Request;
            var clientType = request?.Headers[ClientTypeHeader].FirstOrDefault();

            // X-Client-Type header'ı gelmediyse ve istek trusted origin'lerden geliyorsa,
            // bu isteği Web istemcisi olarak kabul et.
            if (string.IsNullOrWhiteSpace(clientType))
            {
                var origin = request?.Headers["Origin"].FirstOrDefault();
                var webClientOptions = controller.HttpContext.RequestServices.GetRequiredService<IOptions<WebClientOptions>>().Value;

                if (!string.IsNullOrWhiteSpace(origin) &&
                    webClientOptions.TrustedOrigins.Any(o => origin.Contains(o, StringComparison.OrdinalIgnoreCase)))
                {
                    clientType = WebClient;
                }
            }

            // İstemci Web değilse ya da header yoksa doğrudan orijinal response'u dön
            if (!string.Equals(clientType, WebClient, StringComparison.OrdinalIgnoreCase))
            {
                return response;
            }

            var data = (object)response.Data;
            var dataType = data.GetType();

            var accessTokenProperty = dataType.GetProperty("AccessToken");
            var refreshTokenProperty = dataType.GetProperty("RefreshToken");

            if (accessTokenProperty is null ||
                refreshTokenProperty is null ||
                accessTokenProperty.PropertyType != typeof(string) ||
                refreshTokenProperty.PropertyType != typeof(string))
            {
                return response;
            }

            var accessToken = accessTokenProperty.GetValue(data) as string;
            var refreshToken = refreshTokenProperty.GetValue(data) as string;

            // Token değerleri boşsa cookie yazma
            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return response;
            }

            // Web client için CSRF token üret
            var csrfTokenService = controller.HttpContext.RequestServices.GetRequiredService<ICsrfTokenService>();
            var csrfToken = csrfTokenService.GenerateToken();

            var cookieTokenOptions = controller.HttpContext.RequestServices.GetRequiredService<IOptions<CookieTokenOptions>>().Value;
            var csrfOptions = controller.HttpContext.RequestServices.GetRequiredService<IOptions<CsrfOptions>>().Value;

            CookieHelper.SetAuthCookies(controller.Response, accessToken, refreshToken, cookieTokenOptions, csrfOptions, csrfToken);

            // Body'den token'ları temizle
            accessTokenProperty.SetValue(data, null);
            refreshTokenProperty.SetValue(data, null);

            return response;
        }
    }
}
