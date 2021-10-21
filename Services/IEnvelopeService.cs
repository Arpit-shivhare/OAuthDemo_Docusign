using OAuthDemo_Docusign.Models;
using OAuthDemo_Docusign.Services.Responses;

namespace OAuthDemo_Docusign.Services
{
    public interface IEnvelopeService
    {
        CreateEnvelopeResponse CreateEnvelope(
            RequestEnvelopeModel model, string pingAction);
    }
}
