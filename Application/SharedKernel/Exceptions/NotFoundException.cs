using Domain.Enums;

namespace ApplicationService.SharedKernel.Exceptions
{
    public class NotFoundException : Exception
    {
        public ErrorCodes Code { get; }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, ErrorCodes code)
            : base(message)
        {
            Code = code;
        }
    }
}
