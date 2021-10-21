using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OAuthDemo_Docusign.Services.HMACValidation
{
    public class HMACValidationService : IHMACValidationService
    {
        public byte[] ComputeHash(string secret, string payload)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(secret);
            HMAC hmac = new HMACSHA256(bytes);
            bytes = Encoding.UTF8.GetBytes(payload);
            return hmac.ComputeHash(bytes);

        }

        public bool HashIsValid(string secret, string payload, byte[] verifyBytes)
        {
            ReadOnlySpan<byte> hashBytes = ComputeHash(secret, payload);
            return CryptographicOperations.FixedTimeEquals(hashBytes, verifyBytes);
        }
    }
}
