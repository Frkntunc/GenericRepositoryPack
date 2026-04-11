using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Options
{
    public class SecurityHeaderOptions
    {
        public string DevelopmentCsp { get; set; } = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self' ws://localhost:* wss://localhost:*;";
        public string ProductionCsp { get; set; } = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self'; connect-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self'; form-action 'self';";
    }
}
