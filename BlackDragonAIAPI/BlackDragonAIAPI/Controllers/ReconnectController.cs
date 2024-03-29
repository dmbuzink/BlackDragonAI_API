using BlackDragonAIAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackDragonAIAPI.Controllers
{
    [Route("api/reconnect")]
    public class ReconnectController : ControllerBase
    {
        private readonly WebhookManager _webhookManager;

        public ReconnectController(WebhookManager webhookManager)
        {
            _webhookManager = webhookManager;
        }


        [HttpPost]
        public ActionResult Reconnect()
        {
            if (!HttpContext.MeetsAuthorizationLevel(EAuthorizationLevel.ADMIN))
                return Unauthorized(new UnauthorizedError("Incorrect authorization level"));

            this._webhookManager.SendUpdateNotification("/reconnect");
            return Ok();
        }
    }
}
