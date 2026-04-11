namespace ApplicationService.Services.Common
{
    public interface ICsrfTokenService
    {
        string GenerateToken();
        bool ValidateToken(string token);
    }
}
