using Domain.Enums;

namespace ApplicationService.SharedKernel.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public ErrorCodes Code { get; }

        public UnauthorizedException(string message)
            : base(message)
        {
        }

        public UnauthorizedException(string message, ErrorCodes code)
            : base(message)
        {
            Code = code;
        }
    }
}
