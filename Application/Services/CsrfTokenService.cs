using ApplicationService.Services.Common;
using Microsoft.Extensions.Options;
using Shared.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ApplicationService.Services
{
    public class CsrfTokenService : ICsrfTokenService, ISingletonService
    {
        private readonly byte[] _secretKey;

        public CsrfTokenService(IOptions<CsrfOptions> options)
        {
            _secretKey = Encoding.UTF8.GetBytes(options.Value.SecretKey);
        }

        public string GenerateToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            var randomPart = Convert.ToBase64String(randomBytes);

            var signature = ComputeHmac(randomPart);

            return $"{randomPart}.{signature}";
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var dotIndex = token.IndexOf('.');
            if (dotIndex < 0 || dotIndex == token.Length - 1)
                return false;

            var randomPart = token[..dotIndex];
            var signature = token[(dotIndex + 1)..];

            var expectedSignature = ComputeHmac(randomPart);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(signature),
                Encoding.UTF8.GetBytes(expectedSignature));
        }

        private string ComputeHmac(string data)
        {
            using var hmac = new HMACSHA256(_secretKey);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}
