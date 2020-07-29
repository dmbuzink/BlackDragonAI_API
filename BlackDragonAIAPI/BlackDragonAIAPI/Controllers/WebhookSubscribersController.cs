using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using BlackDragonAIAPI.StorageHandlers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BlackDragonAIAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/webhook")]
    [ApiController]
    public class WebhookSubscribersController : ControllerBase
    {
        private IWebhookSubscriberService _db { get; set; }

        public WebhookSubscribersController(IWebhookSubscriberService db)
        {
            this._db = db;
        }

        [HttpPost]
        public async Task<ActionResult<WebhookSubscriber>> CreateWebhookSubscriber()
        {
            if ((await this._db.GetWebhookSubscribers()).Any(dbWs => dbWs.Uri.Equals(GetClientAddress())))
                return BadRequest(new BadRequestError("A webhook subscriber has already been created for this uri"));
            return await AddWebhookSubscriberToDatabase();
        }

        /// <summary>
        /// Creates a webhook subscriber in an idempotent manner. Will always ensure resource exists if the request is valid
        /// </summary>
        [HttpPost("idempotent")]
        public async Task<ActionResult<WebhookSubscriber>> CreateWebhookSubscriberIdempotent() =>
            (await this._db.GetWebhookSubscribers()).FirstOrDefault(dbWs => dbWs.Uri.Equals(GetClientAddress())) 
            ?? await AddWebhookSubscriberToDatabase();

        private async Task<WebhookSubscriber> AddWebhookSubscriberToDatabase()
        {
            var ws = new WebhookSubscriber()
            {
                Guid = Guid.NewGuid(),
                Uri = GetClientAddress()
            };
            return await this._db.CreateWebhookSubscriber(ws);
        }

        private string GetClientAddress() =>
            HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
//            HttpContext.Connection.RemoteIpAddress.ToString();
    }
}
