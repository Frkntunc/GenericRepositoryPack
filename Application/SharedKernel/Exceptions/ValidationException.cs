using Domain.Enums;

namespace ApplicationService.SharedKernel.Exceptions
{
    public class ValidationException : Exception
    {
        public string MessageText { get; }
        public ErrorCodes Code { get; }

        public ValidationException(string messageText, ErrorCodes code)
            : base(messageText)
        {
            MessageText = messageText;
            Code = code;
        }
    }

}
