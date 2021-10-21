using DocuSign.eSign.Model;

namespace OAuthDemo_Docusign.Services.TemplateHandler
{
    public interface ITemplateHandler
    {
        EnvelopeTemplate BuildTemplate(string rootDir);
        EnvelopeDefinition BuildEnvelope(string currentUserEmail, string currentUserFullName, string receiverEmail, string receiverFullName);
        string TemplateName { get; }

    }
}
