using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Auth
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
