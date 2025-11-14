using Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class ResponseCodeMapper
    {
        private static readonly Dictionary<string, int> Map = new()
    {
        { ResponseCodes.Success, 200 },

        { ResponseCodes.ValidationError, 400 },
        //{ ResponseCodes.NotFound, 404 },
        //{ ResponseCodes.Conflict, 409 },

        //{ ResponseCodes.Unauthorized, 401 },
        //{ ResponseCodes.Forbidden, 403 },

        //{ ResponseCodes.InternalError, 500 },
        //{ ResponseCodes.ProviderError, 502 },  // ödeme provider error
    };

        public static int GetHttpStatus(string code)
        {
            if (code == null)
                return 500;

            if (Map.TryGetValue(code, out var status))
                return status;

            return 500; // default fallback
        }
    }
}
