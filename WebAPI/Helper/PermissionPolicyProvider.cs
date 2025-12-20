using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using WebAPI.Auth;

namespace WebAPI.Helper
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // Standart policy provider'ı (örn: [Authorize] için) yedekte tutuyoruz.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
            FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Eğer politika adı "Permission" ile başlamıyorsa standart işleyişe bırak
            // Ancak biz Attribute içinde direkt permission adını göndereceğiz, 
            // bu yüzden her bilinmeyen politikayı bir PermissionRequirement olarak kabul ediyoruz.

            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(policyName));
            return Task.FromResult(policy.Build())!;
        }
    }

}
