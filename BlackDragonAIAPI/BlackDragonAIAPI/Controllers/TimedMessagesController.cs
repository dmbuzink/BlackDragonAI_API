using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using BlackDragonAIAPI.Models.Validation;
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
        private readonly TimedMessageValidator _validator;

        public TimedMessagesController(ITimedMessageService timedMessageService, TimedMessageValidator validator, ICommandService commandService, WebhookManager webhookManager)
        {
            this._timedMessageService = timedMessageService;
            this._commandService = commandService;
            this._webhookManager = webhookManager;
            this._validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<TimedMessage>> CreateTimedMessage(TimedMessage timedMessage)
        {
            var validationResult = await _validator.ValidateAsync(timedMessage);
            if (!validationResult.IsValid)
                return BadRequest(new BadRequestError(validationResult.ToString(" & ")));

            var command = await _commandService.GetCommand(c => c.Command.Equals(timedMessage.Command));
            if (command == null)
                return BadRequest(new BadRequestError("Specified command does not exist"));

            if ((await _timedMessageService.GetTimedMessages(c => c.Command.Equals(timedMessage.Command))).Any())
                return BadRequest(new BadRequestError("Timed message already exists"));

            var dbTimedMessage = new TimedMessage()
            {
                Guid = Guid.NewGuid(),
                Command = command.OriginalCommand,
                IntervalInMinutes = timedMessage.IntervalInMinutes,
                OffsetInMinutes = timedMessage.OffsetInMinutes
            };
            await this._timedMessageService.AddTimedMessage(dbTimedMessage);
            this._webhookManager.SendUpdateNotification("/timedmessages");
            return dbTimedMessage;
        }

        [HttpPut("{command}")]
        public async Task<ActionResult<TimedMessage>> UpdateTimedMessage(TimedMessage timedMessage, string command)
        {
            timedMessage.Command = command;
            var validationResult = await _validator.ValidateAsync(timedMessage);
            if (!validationResult.IsValid)
                return BadRequest(new BadRequestError(validationResult.ToString(" & ")));

            var dbCommand = await _commandService.GetCommand(c => c.Command.Equals(timedMessage.Command));
            if (dbCommand == null)
                return BadRequest(new BadRequestError("Specified command does not exist"));

            var dbTimedMessage = await _timedMessageService.GetTimedMessage(c => c.Command.Equals(timedMessage.Command));
            if (dbTimedMessage == null)
                return BadRequest(new BadRequestError("Timed message does not yet exist"));

            dbTimedMessage.IntervalInMinutes = timedMessage.IntervalInMinutes;
            dbTimedMessage.OffsetInMinutes = timedMessage.OffsetInMinutes;

            await this._timedMessageService.UpdateTimedMessages(tm => tm.Guid.Equals(dbTimedMessage.Guid),
                tm => dbTimedMessage);
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
