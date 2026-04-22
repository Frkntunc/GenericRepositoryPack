using ApplicationService.Services.Common;
using ApplicationService.SharedKernel.Auth.Common;
using System.Security.Cryptography;
using System.Text;

namespace ApplicationService.SharedKernel.Auth
{
    public class HashService : IHashService, IScopedService
    {
        private static readonly string PasswordSalt = "PasswordSaltSecretKey";
        public string Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public string HashWithSalt(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var byteSalt = Encoding.UTF8.GetBytes(PasswordSalt);
                var combined = new byte[bytes.Length + byteSalt.Length];
                Buffer.BlockCopy(bytes, 0, combined, 0, bytes.Length);
                Buffer.BlockCopy(byteSalt, 0, combined, bytes.Length, byteSalt.Length);
                var hash = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
