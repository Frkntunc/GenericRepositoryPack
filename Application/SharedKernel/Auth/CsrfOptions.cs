using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.SharedKernel.Auth
{
    public class CsrfOptions
    {
        public string CookieName { get; set; } = "XSRF-TOKEN";
        public string HeaderName { get; set; } = "X-CSRF-TOKEN";
        public List<string> MethodsToCheck { get; set; } = new List<string>();
        public List<string> ExemptPaths { get; set; } = new List<string>();
    }

}
