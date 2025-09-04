using Shared.Enums;

namespace Shared.Exceptions
{
    public class UnauthorizedException : BaseAppException
    {
        public UnauthorizedException(ErrorCodes code)
            : base(code)
        {
        }
    }
}
