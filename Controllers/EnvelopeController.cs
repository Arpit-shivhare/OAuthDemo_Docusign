using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAuthDemo_Docusign.Models;
using OAuthDemo_Docusign.Services;
using OAuthDemo_Docusign.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthDemo_Docusign.Controllers
{
     [EnableCors("AllowAllOrigins")]
     [Route("api/[controller]")]

    public class EnvelopeController : Controller
    {
        private readonly IEnvelopeService _envelopeService;

        public EnvelopeController(IEnvelopeService envelopeService)
        {
            _envelopeService = envelopeService;
        }

        [HttpPost]
        public IActionResult Index([FromBody] RequestEnvelopeModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }
            string scheme = Url.ActionContext.HttpContext.Request.Scheme;

            CreateEnvelopeResponse createEnvelopeResponse = _envelopeService.CreateEnvelope(model, Url.Action("ping", "info", null, scheme));

            //CreateEnvelopeResponse createEnvelopeResponse = _envelopeService.CreateEnvelope(
            //    model.Type,
            //    model.AccountId,
            //    //Context.User.Id,
            //    Context.User.LoginType,
            //    model.AdditionalUser,
            //    model.RedirectUrl,
            //    Url.Action("ping", "info", null, scheme));

            return Ok(new ResponseEnvelopeModel
            {
                RedirectUrl = createEnvelopeResponse.RedirectUrl,
                EnvelopeId = createEnvelopeResponse.EnvelopeId
            });
        }
    }
}
