using Shared.Enums;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public abstract class BaseAppException : Exception
    {
        public ErrorCodes ErrorCode { get; }

        protected BaseAppException(ErrorCodes errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
