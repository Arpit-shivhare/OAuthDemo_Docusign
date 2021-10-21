using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthDemo_Docusign.Services.HMACValidation
{
    public interface IHMACValidationService
    {
        byte[] ComputeHash(string secret, string payload);

        bool HashIsValid(string secret, string payload, byte[] verifyBytes);
    }
}
