using Microsoft.Extensions.Options;
using WebAPI.Middleware;

namespace WebAPI.Extensions
{
    public static class XssProtectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseXssProtection(this IApplicationBuilder builder, Action<XssProtectionOptions>? configureOptions = null)
        {
            var options = new XssProtectionOptions();
            configureOptions?.Invoke(options);
            return builder.UseMiddleware<XssProtectionMiddleware>(Options.Create(options));
        }
    }
}
