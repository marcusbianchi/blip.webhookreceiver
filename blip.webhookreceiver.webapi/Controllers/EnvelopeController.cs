using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using blip.webhookreceiver.core.Services;
using blip.webhookreceiver.core.Interfaces;
using System.IO;
using System.Text;
using blip.webhookreceiver.core.Models;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvelopeController : ControllerBase
    {
        private readonly IReceiveLimeMessage _receiveLimeMessage;
        public EnvelopeController(IReceiveLimeMessage receiveLimeMessage)
        {
            _receiveLimeMessage = receiveLimeMessage;
        }
        
        /// <summary>
        /// Receives Webhook posts
        /// </summary>
        /// <param name="json">Blip JSON</param>
        /// <returns></returns>
        /// <response code="400">Type Incompability</response>
        /// <response code="200">Response Ingested</response>
        [HttpPost]
        [Consumes("application/json")]
        public IActionResult Post(JObject json)
        {
            if (json["from"] != null)
            {
                _receiveLimeMessage.ProcessMessage(json);
                return Ok();
            }
            else if (json["ownerIdentity"] != null)
            {
                _receiveLimeMessage.ProcessEvent(json);
                return Ok();
            }
            return BadRequest();
        }
    }
}