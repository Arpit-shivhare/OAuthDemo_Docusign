using DocuSign.eSign.Api;
using System.Net.Http;

namespace OAuthDemo_Docusign.Services
{
    public interface IDocuSignApiProvider
    {
        IUsersApi UsersApi { get; }
        IEnvelopesApi EnvelopApi { get; }
        HttpClient DocuSignHttpClient { get; }
        ITemplatesApi TemplatesApi { get; }
        IAccountsApi AccountsApi { get; }
    }
}
