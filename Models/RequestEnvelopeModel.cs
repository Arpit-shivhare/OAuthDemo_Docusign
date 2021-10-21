using OAuthDemo_Docusign.Domain;

namespace OAuthDemo_Docusign.Models
{
    public class RequestEnvelopeModel
    {
        public DocumentType Type { get; set; }

        public string AccountId { get; set; }

        public string ReceiverEmail { get; set; }
        public string ReceiverFullName { get; set; }
        public string SendersFullName { get; set; }
        public string SendersEmail { get; set; }

        public string RedirectUrl { get; set; }
    }
}
