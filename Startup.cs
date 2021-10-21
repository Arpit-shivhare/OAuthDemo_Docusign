using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OAuthDemo_Docusign.Domain;
using OAuthDemo_Docusign.Models;
using OAuthDemo_Docusign.Services;
using OAuthDemo_Docusign.Services.HMACValidation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthDemo_Docusign
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpContextAccessor();

            services.AddScoped<IDocuSignApiProvider, DocuSignApiProvider>();

            services.AddHttpClient<DocuSignApiProvider>();

            services.AddScoped<IEnvelopeService, EnvelopeService>();

            services.AddScoped<IHMACValidationService, HMACValidationService>();

            // Adding OAuth
            services.AddAuthentication(options =>
            {
                //options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "DocuSign";
            }
            ).AddCookie()
            .AddOAuth("DocuSign", options =>
            {
                options.ClientId = "61087d8f-fd93-4c96-8946-bdc2339d0bf1";
                options.ClientSecret = "aa135a33-ca33-4511-b098-973a71ae6033";
                options.CallbackPath = new PathString("/ds/callback");
                options.AuthorizationEndpoint = Configuration["DocuSign:AuthorizationEndpoint"];
                options.TokenEndpoint = Configuration["DocuSign:TokenEndpoint"];
                options.UserInformationEndpoint = Configuration["DocuSign:UserInformationEndpoint"];


                options.Scope.Add("signature");
                options.Scope.Add("click.manage");
                options.SaveTokens = true;

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("accounts", "accounts");
                options.ClaimActions.MapJsonKey("authType", "authType");
                //options.ClaimActions.MapCustomJson("account_id", obj => ExtractDefaultAccountValue(obj, "account_id"));


                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Add("X-Content-Type-Options", "nosniff");
                        request.Headers.Add("X-Frame-Options", "deny");
                        request.Headers.Add("Cache-Control", "no-cache");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());
                        user.Add("authType", LoginType.CodeGrant.ToString());

                        //using JsonDocument payload = JsonDocument.Parse(user.ToString());

                        context.RunClaimActions(user);
                    },
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = $"Internal Server Error: {context.Failure.Message}"
                        }.ToString());
                        return Task.FromResult(0);
                    },
                    OnRedirectToAuthorizationEndpoint = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.Headers["Location"] = context.RedirectUri;
                            context.Response.StatusCode = 401;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }

                        return Task.CompletedTask;
                    }
                };

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes => 
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
