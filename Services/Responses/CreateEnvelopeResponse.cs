using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthDemo_Docusign.Services.Responses
{
    public class CreateEnvelopeResponse
    {
        public CreateEnvelopeResponse(string redirectUrl, string envelopeId)
        {
            RedirectUrl = redirectUrl;
            EnvelopeId = envelopeId;
        }

        public string RedirectUrl { get; }
        public string EnvelopeId { get; }
    }
}
