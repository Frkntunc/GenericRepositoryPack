using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Constants
{
    public static class ResponseCodes
    {
        public const string Success = "0000";
        public const string UnexpectedError = "1001";
        public const string ValidationError = "1002";
        public const string UserNotFound = "3000";
        public const string InvalidRefreshToken = "2000";
    }
}
