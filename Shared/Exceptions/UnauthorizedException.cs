using Shared.Enums;

namespace Shared.Exceptions
{
    public class UnauthorizedException : BaseAppException
    {
        public UnauthorizedException(string code)
            : base(code)
        {
        }
    }
}
