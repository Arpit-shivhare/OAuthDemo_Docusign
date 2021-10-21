using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OAuthDemo_Docusign.Services
{
    public class DocuSignApiProvider : IDocuSignApiProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public DocuSignApiProvider(IHttpContextAccessor httpContextAccessor,
                                   IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }


        private Lazy<IUsersApi> _usersApi => new Lazy<IUsersApi>(() => new UsersApi(_apiClient.Value));
        private Lazy<IEnvelopesApi> _envelopApi => new Lazy<IEnvelopesApi>(() => new EnvelopesApi(_apiClient.Value));
        private Lazy<ITemplatesApi> _templatesApi => new Lazy<ITemplatesApi>(() => new TemplatesApi(_apiClient.Value));
        private Lazy<IAccountsApi> _accountsApi => new Lazy<IAccountsApi>(() => new AccountsApi(_apiClient.Value));



        private Lazy<HttpClient> _docuSignHttpClient => new Lazy<HttpClient>(() =>
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiClient.Value.Configuration.AccessToken);
            client.BaseAddress = new Uri("https://demo.docusign.net");
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        });



        private Lazy<ApiClient> _apiClient => new Lazy<ApiClient>(() =>
        {
            var docuSignConfig = new Configuration("https://demo.docusign.net" + "/restapi");
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
            string token = "";

            var authorization = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                // we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;

                token = parameter;
                // scheme will be "Bearer"
                // parmameter will be the token itself.
            }
            

           
            if (string.IsNullOrEmpty(token))
            {
                token = _httpContextAccessor.HttpContext.User.FindAll("access_token").First().Value;
            }
            docuSignConfig.AddDefaultHeader("Authorization", accessToken);
            docuSignConfig.AccessToken = token;
            var apiClient = new ApiClient(docuSignConfig);
            apiClient.SetBasePath(docuSignConfig.BasePath);
            return apiClient;

        });




         public IUsersApi UsersApi => _usersApi.Value;
        public IEnvelopesApi EnvelopApi => _envelopApi.Value;
        public HttpClient DocuSignHttpClient => _docuSignHttpClient.Value;
        public ITemplatesApi TemplatesApi => _templatesApi.Value;
        public IAccountsApi AccountsApi => _accountsApi.Value;

    }
}
