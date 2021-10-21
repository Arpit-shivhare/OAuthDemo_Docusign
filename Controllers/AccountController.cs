using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAuthDemo_Docusign.Models;
using OAuthDemo_Docusign.Services;
using OAuthDemo_Docusign.Services.Responses;

namespace OAuthDemo_Docusign.Controllers
{
    [EnableCors("AllowAllOrigins")]
    public class AccountController : Controller
    {
        private readonly IEnvelopeService _envelopeService;

        public AccountController(IEnvelopeService envelopeService)
        {
            _envelopeService = envelopeService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }


        [HttpPost]
        public IActionResult SendEnvelope([FromBody] RequestEnvelopeModel model)
         {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }
            string scheme = Url.ActionContext.HttpContext.Request.Scheme;

            CreateEnvelopeResponse createEnvelopeResponse = _envelopeService.CreateEnvelope(model, Url.Action("ping", "info", null, scheme));

            return Ok(new ResponseEnvelopeModel
            {
                RedirectUrl = createEnvelopeResponse.RedirectUrl,
                EnvelopeId = createEnvelopeResponse.EnvelopeId
            });
        }
    }
}
