using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using OAuthDemo_Docusign.Domain;
using OAuthDemo_Docusign.Models;
using OAuthDemo_Docusign.Services.Responses;
using OAuthDemo_Docusign.Services.TemplateHandler;
using System;
using System.Linq;

namespace OAuthDemo_Docusign.Services
{
    public class EnvelopeService : IEnvelopeService
    {
        private string _signerClientId = "1000";
        private readonly IConfiguration _configuration;
        private readonly IDocuSignApiProvider _docuSignApiProvider;

        public EnvelopeService(IConfiguration configuration, IDocuSignApiProvider docuSignApiProvider)
        {
            _configuration = configuration;
            _docuSignApiProvider = docuSignApiProvider;
        }

        public CreateEnvelopeResponse CreateEnvelope(RequestEnvelopeModel model, string pingAction)
        {
            if (model.AccountId == null)
            {
                throw new ArgumentNullException(nameof(model.AccountId));
            }
            //if (userId == null)
            //{
            //    throw new ArgumentNullException(nameof(userId));
            //}

            string rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            var templateHandler = GetTemplateHandler(model.Type);

            EnvelopeTemplate envelopeTemplate = templateHandler.BuildTemplate(rootDir);

            EnvelopeDefinition envelope = templateHandler.BuildEnvelope(model.SendersEmail, model.SendersFullName, model.ReceiverEmail, model.ReceiverFullName);

            var listTemplates = _docuSignApiProvider.TemplatesApi.ListTemplates(model.AccountId);

            EnvelopeTemplate template = listTemplates?.EnvelopeTemplates?.FirstOrDefault(x => x.Name == templateHandler.TemplateName);

            if (template != null)
            {
                envelope.TemplateId = template.TemplateId;
            }
            else
            {
                TemplateSummary templateSummary = _docuSignApiProvider.TemplatesApi.CreateTemplate(model.AccountId, envelopeTemplate);

                envelope.TemplateId = templateSummary.TemplateId;
            }

            EnvelopeSummary envelopeSummary = _docuSignApiProvider.EnvelopApi.CreateEnvelope(model.AccountId, envelope);

            if (model.Type != DocumentType.I9)
            {
                ViewUrl recipientView = _docuSignApiProvider.EnvelopApi.CreateRecipientView(
                    model.AccountId,
                    envelopeSummary.EnvelopeId,
                    BuildRecipientViewRequest(
                        model.SendersEmail,
                        model.SendersFullName,
                        model.RedirectUrl,
                        pingAction)
                    );
                return new CreateEnvelopeResponse(recipientView.Url, envelopeSummary.EnvelopeId);
            }

            return new CreateEnvelopeResponse(string.Empty, envelopeSummary.EnvelopeId);

        }


        private RecipientViewRequest BuildRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string pingUrl)
        {
            RecipientViewRequest viewRequest = new RecipientViewRequest
            {
                ReturnUrl = returnUrl,
                AuthenticationMethod = "none",
                Email = signerEmail,
                UserName = signerName,
                ClientUserId = _signerClientId
            };

            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600";
                viewRequest.PingUrl = pingUrl;
            }

            return viewRequest;
        }




        private ITemplateHandler GetTemplateHandler(DocumentType type)
        {
            ITemplateHandler templateHandler;
            switch (type)
            {
               
                case DocumentType.Offer:
                    templateHandler = new OfferTemplateHandler();
                    break;
              
                default:
                    throw new InvalidOperationException("Document type is not set");
            }

            return templateHandler;
        }

    }
}
