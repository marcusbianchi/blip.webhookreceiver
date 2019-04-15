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
using Microsoft.Extensions.Logging;

namespace blip.webhookreceiver.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvelopeController : ControllerBase
    {
        private readonly IReceiveLimeMessage _receiveLimeMessage;
        private readonly ILogger _logger;

        public EnvelopeController(IReceiveLimeMessage receiveLimeMessage, ILogger<EnvelopeController> logger)
        {
            _receiveLimeMessage = receiveLimeMessage;
            _logger = logger;
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
        public async Task<IActionResult> Post(JObject json)
        {
            if (json["from"] != null)
            {
                await _receiveLimeMessage.ProcessMessage(json);
                _logger.LogInformation("HTTP Message Processed");
                return Ok();
            }
            else if (json["ownerIdentity"] != null)
            {
                await _receiveLimeMessage.ProcessEvent(json);
                _logger.LogInformation("HTTP Event Processed");
                return Ok();
            }
            _logger.LogWarning("HTTP JSON not Recognized. JSON: {json}",JsonConvert.SerializeObject(json));
            return BadRequest();
        }
    }
}