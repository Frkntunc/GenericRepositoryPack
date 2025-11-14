using Shared.Enums;

namespace Shared.Exceptions
{
    public class ValidationException : BaseAppException
    {
        public ValidationException(string code)
            : base(code)
        {
        }
    }

}
