using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Options
{
    public class AuthMiddlewareOptions
    {
        public string[] AnonymousPaths { get; set; } =
        [
            "/swagger",
            "/swagger/index.html",
            "/swagger/v1/swagger.json",
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/sendotp",
            "/api/auth/verifyotp"
        ];
    }
}
