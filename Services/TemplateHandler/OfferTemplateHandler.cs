using DocuSign.eSign.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace OAuthDemo_Docusign.Services.TemplateHandler
{
    public class OfferTemplateHandler : ITemplateHandler
    {

        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/EmploymentOfferLetter.json";
        public string TemplateName => "Employment Offer Letter Sample";

        public EnvelopeDefinition BuildEnvelope(string currentUserEmail, string currentUserFullName, string receiverEmail, string receiverFullName)
        {
            var roleHr = new TemplateRole
            {
                Email = currentUserEmail,
                Name = currentUserFullName,
                ClientUserId = _signerClientId,
                RoleName = "HR Rep"
            };
            Signer
            var roleNewHire = new TemplateRole
            {
                Email = receiverEmail,
                Name = receiverFullName,
                RoleName = "New Hire"
            };

            var env = new EnvelopeDefinition
            {
                TemplateRoles = new List<TemplateRole> { roleHr, roleNewHire },
                Status = "sent"
            };
            return env;
        }

        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            using (var reader = new StreamReader(rootDir + _templatePath))
            {
               return JsonConvert.DeserializeObject<EnvelopeTemplate>(reader.ReadToEnd());
            }
        }
    }
}
