using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthDemo_Docusign.Services.HMACValidation;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OAuthDemo_Docusign.Controllers
{
    [EnableCors("AllowAllOrigins")]
    public class WebhookController : Controller
    {


        private string HMacSecret = "vnRlFmFp+zNhkY71sYLJIeIlXjBPnpq6K7RIpEQH7Z4";
        private readonly IHMACValidationService _hMACValidationService;


        public WebhookController(IHMACValidationService hMACValidationService)
        {
            _hMACValidationService = hMACValidationService;
        }


        [HttpPost]
        public async Task<ActionResult> HandleEvent()
          {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var headerVal = HttpContext.Request.Headers["X-DocuSign-Signature-1"];

            var val = _hMACValidationService.ComputeHash(HMacSecret, headerVal);

            bool valid = _hMACValidationService.HashIsValid(HMacSecret, headerVal, val);

            if(valid == true)
            {
                return Ok(json);
            }
            else
            {
                return Unauthorized();  
            }

        }
    }
}
