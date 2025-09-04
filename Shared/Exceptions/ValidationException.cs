using Shared.Enums;

namespace Shared.Exceptions
{
    public class ValidationException : BaseAppException
    {
        public ValidationException(ErrorCodes code)
            : base(code)
        {
        }
    }

}
