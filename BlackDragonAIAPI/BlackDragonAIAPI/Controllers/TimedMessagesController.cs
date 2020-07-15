using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using BlackDragonAIAPI.StorageHandlers;
using Microsoft.AspNetCore.Mvc;

namespace BlackDragonAIAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimedMessagesController : ControllerBase
    {
        private readonly ITimedMessageService _timedMessageService;
        private readonly ICommandService _commandService;
        private readonly WebhookManager _webhookManager;

        public TimedMessagesController(ITimedMessageService timedMessageService, ICommandService commandService, WebhookManager webhookManager)
        {
            this._timedMessageService = timedMessageService;
            this._commandService = commandService;
            this._webhookManager = webhookManager;
        }

        [HttpPost]
        public async Task<ActionResult<TimedMessage>> CreateTimedMessage(TimedMessage timedMessage)
        {
            if (!CommandDetailsExtensions.GetCommandRegex().IsMatch(timedMessage.Command))
                return BadRequest(new BadRequestError("Invalid command"));

            var command = await _commandService.GetCommand(c => c.Command.Equals(timedMessage.Command));
            if (command == null)
                return BadRequest(new BadRequestError("Specified command does not exist"));

            if ((await _timedMessageService.GetTimedMessages(c => c.Command.Equals(timedMessage.Command))).Any())
                return BadRequest(new BadRequestError("Timed message already exists"));

            var dbTimedMessage = new TimedMessage()
            {
                Guid = Guid.NewGuid(),
                Command = command.OriginalCommand
            };
            await this._timedMessageService.AddTimedMessage(dbTimedMessage);
            this._webhookManager.SendUpdateNotification("/timedmessages");
            return dbTimedMessage;
        }

        [HttpGet]
        public async Task<IEnumerable<TimedMessage>> GetTimedMessages() =>
            await this._timedMessageService.GetTimedMessages();

        [HttpDelete("{commandName}")]
        public async Task<ActionResult> DeleteTimedMessage(string commandName)
        {
            var command = await _commandService.GetCommand(c => c.Command.Equals(commandName.ToLower()));
            if (command == null)
                return NoContent();

            if (!(await _timedMessageService.GetTimedMessages(tm => tm.Command.Equals(command.OriginalCommand))).Any())
                return NoContent();

            await this._timedMessageService.DeleteTimedMessage(commandName);
            this._webhookManager.SendUpdateNotification("/timedmessages");
            return NoContent();
        }
    }
}
