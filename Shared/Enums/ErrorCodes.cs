using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum ErrorCodes
    {
        Unknown = 0,

        // Genel
        ValidationError = 1001,
        NotFound = 1002,
        UnexpectedError = 1003,

        // Kullanıcı
        UserNotFound = 2001,
        InvalidLogin = 2002,

        // Yetkilendirme
        Unauthorized = 3001,
        Forbidden = 3002,
        InvalidRefreshToken = 3003
    }

}
