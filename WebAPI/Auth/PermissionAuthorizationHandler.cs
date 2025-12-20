using Microsoft.AspNetCore.Authorization;
using Shared.Constants;
using Shared.Exceptions;

namespace WebAPI.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Kullanıcı giriş yapmamışsa reddet
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException(ResponseCodes.UnauthorizedError);
            }

            // JWT üretirken claim adını "permission" olarak vermiştiniz.
            // Burada kullanıcının claimlerinde bu izin var mı diye bakıyoruz.
            var hasPermission = context.User.Claims.Any(c =>
                c.Type == "permission" &&
                c.Value == requirement.Permission);

            if (!hasPermission)
            {
                throw new UnauthorizedException(ResponseCodes.UnauthorizedError);
            }

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

}
