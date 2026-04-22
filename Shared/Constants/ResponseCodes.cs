namespace Shared.Constants
{
    public static class ResponseCodes
    {
        public const string Success = "0000";
        public const string UnexpectedError = "1001";
        public const string ValidationError = "1002";
        public const string UnauthorizedError = "1003";
        public const string UserNotFound = "3000";
        public const string EnterValidEmail = "3011";
        public const string EmailCannotBeEmpty = "3012";
        public const string PasswordRequired = "3013";
        public const string IpAddressRequired = "3014";
        public const string EmailOrPasswordWrong = "3020";
        public const string InvalidRefreshToken = "2000";
        public const string InvalidToken = "4001";
        public const string MissingToken = "4002";
        public const string CsrfCookieMissing = "4003";
        public const string CsrfHeaderMissing = "4004";
        public const string CsrfValidationFailed = "4005";
        public const string CsrfSignatureInvalid = "4006";
        public const string IdempotencyKeyMissing = "4007";
        public const string RequestAlreadyProcessing = "4008";
        public const string XssDetectedInBody = "4009";
        public const string XssDetectedInQueryString = "4010";
        public const string RequestBodyTooLarge = "4011";
        public const string InvalidJsonFormat = "4012";
    }
}
