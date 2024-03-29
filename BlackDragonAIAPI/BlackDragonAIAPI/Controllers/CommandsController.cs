using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using BlackDragonAIAPI.Models.Validation;
using BlackDragonAIAPI.StorageHandlers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlackDragonAIAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly Regex _commandRegex;
        private readonly CommandValidator _validator;
        private readonly ICommandService _commandService;
        private readonly ITimedMessageService _timedMessageService;
        private readonly WebhookManager _webhookManager;

        public CommandsController(ICommandService commandService, ITimedMessageService timedMessageService, CommandValidator validator, WebhookManager webhookManager)
        {
            this._commandService = commandService;
            this._timedMessageService = timedMessageService;
            this._validator = validator;
            this._commandRegex = CommandDetailsExtensions.GetCommandRegex();
            this._webhookManager = webhookManager;
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> CreateCommandsBulk(IEnumerable<CommandDetails> commands)
        {
            foreach (var command in commands)
            {
                await CreateCommand(command);
            }

            return Created("", "done");
        }

//        [HttpGet("exists/{commandName}")]
//        public async Task<Existence> CommandExists(string commandName)

        [HttpPost]
        public async Task<ActionResult<CommandDetails>> CreateCommand(CommandDetails commandDetails)
        {
            if (!IsAuthorized()) return Unauthorized(new UnauthorizedError());

            var validationResult = await this._validator.ValidateAsync(commandDetails);
            if (!validationResult.IsValid)
            {
                return BadRequest(new BadRequestError(validationResult.ToString(" & ")));
            }

            commandDetails.Command = commandDetails.Command.ToLower();

            if((await _commandService.GetCommands(commandDb => commandDb.Command.Equals(commandDetails.Command))).Any())
            {
                return BadRequest(new BadRequestError("Command with the same name already exists"));
            }

            commandDetails.OriginalCommand = commandDetails.Command;
            var dbCommand = await this._commandService.CreateCommand(commandDetails);
            this._webhookManager.SendUpdateNotification("/commands");
            return Created("", dbCommand);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandDetails>>> GetCommands() =>
            Ok(await this._commandService.GetCommands());

        [HttpPut]
        public async Task<ActionResult> UpdateCommand(CommandDetails commandDetails)
        {
            if (!IsAuthorized()) return Unauthorized(new UnauthorizedError());

            commandDetails.Command = commandDetails.Command.ToLower();
            var validationResult = await this._validator.ValidateAsync(commandDetails);
            if (!validationResult.IsValid)
            {
                return BadRequest(new BadRequestError(validationResult.ToString(" & ")));
            }

            var currentCommand = await this._commandService.GetCommand(commandDb => commandDb.Command.Equals(commandDetails.Command));
            if (currentCommand == null)
                return BadRequest(new BadRequestError("The command to edit was not found"));

            await this._commandService.UpdateCommands(commandDb => commandDb.OriginalCommand.Equals(currentCommand.OriginalCommand),
                commandDb =>
                {
                    commandDb.Permission = commandDetails.Permission;
                    commandDb.Timer = commandDetails.Timer;
                    commandDb.Message = commandDetails.Message;
                    return commandDb;
                });
            this._webhookManager.SendUpdateNotification("/commands");
            return NoContent();
        }

        [HttpDelete("{command}")]
        public async Task<ActionResult> DeleteCommand([FromRoute] string command)
        {
            if (!IsAuthorized()) return Unauthorized(new UnauthorizedError());

            command = command.ToLower();
            if (command[0] != '!')
                command = $"!{command}";
            var dbCommand = await this._commandService.GetCommand(commandDb => commandDb.Command.Equals(command));
            if (dbCommand != null)
            {
                await this._commandService.DeleteCommands(commandDb => commandDb.OriginalCommand.Equals(dbCommand.OriginalCommand));
                if ((await this._timedMessageService.GetTimedMessages(tm => tm.Command.Equals(dbCommand.OriginalCommand))).Any())
                {
                    await this._timedMessageService.DeleteTimedMessage(dbCommand.OriginalCommand);
                }
                this._webhookManager.SendUpdateNotification("/commands");
            }
            return NoContent();
        }

        [HttpPost("alias/{command}")]
        public async Task<ActionResult<CommandDetails>> CreateAlias([FromRoute] string command, AliasDetails aliasDetails)
        {
            if (!IsAuthorized()) return Unauthorized(new UnauthorizedError());

            command = command.ToLower();
            if (command[0] != '!')
                command = $"!{command}";

            aliasDetails.Alias = aliasDetails.Alias.ToLower();
            if (!_commandRegex.IsMatch(aliasDetails.Alias))
                return BadRequest(new BadRequestError("Alias is invalid"));
            if (!_commandRegex.IsMatch(command))
                return BadRequest(new BadRequestError("Command to make an alias of is invalid"));

            if ((await this._commandService.GetCommands(commandDb => commandDb.Command.Equals(aliasDetails.Alias))).Any())
            {
                return BadRequest(new BadRequestError("Given alias is already used as a command"));
            }

            var commandToMakeAliasOf = await this._commandService.GetCommand(commandDb => commandDb.Command.Equals(command));
            var alias = await this._commandService.CreateCommand(new CommandDetails()
            {
                Command = aliasDetails.Alias,
                Message = commandToMakeAliasOf.Message,
                OriginalCommand = commandToMakeAliasOf.OriginalCommand,
                Permission = commandToMakeAliasOf.Permission,
                Timer = commandToMakeAliasOf.Timer
            });
            this._webhookManager.SendUpdateNotification("/commands");
            return Created("", alias);
        }

        [HttpDelete("alias/{command}")]
        public async Task<ActionResult> DeleteAlias([FromRoute] string command)
        {
            if (!IsAuthorized()) return Unauthorized(new UnauthorizedError());

            command = command.ToLower();
            if (command[0] != '!')
                command = $"!{command}";
            if (!_commandRegex.IsMatch(command))
                return BadRequest(new BadRequestError("Invalid command"));

            var dbCommand = await this._commandService.GetCommand(commandDb => commandDb.Command.Equals(command));
            if (dbCommand == null)
                return NoContent();

            var amountOfAliases = (await this._commandService.GetCommands(commandDb =>
                commandDb.OriginalCommand.Equals(dbCommand.OriginalCommand))).Count();
            if (dbCommand.Command.Equals(dbCommand.OriginalCommand) && amountOfAliases > 1)
            {
                // Original command needs to be changed first
                var aliasOfCommandToDelete = await this._commandService.GetCommand(commandDb =>
                    commandDb.OriginalCommand.Equals(dbCommand.OriginalCommand) && !commandDb.Command.Equals(command));
                await this._commandService.UpdateCommands(
                    commandDb => commandDb.OriginalCommand.Equals(dbCommand.OriginalCommand),
                    commandDb =>
                    {
                        commandDb.OriginalCommand = aliasOfCommandToDelete.Command;
                        return commandDb;
                    });

                await this._timedMessageService.UpdateTimedMessages(tm => tm.Command.Equals(command), tm =>
                {
                    tm.Command = aliasOfCommandToDelete.Command;
                    return tm;
                });
            }

            if (amountOfAliases <= 1)
            {
                await this._timedMessageService.DeleteTimedMessage(command);
            }

            await this._commandService.DeleteCommands(commandDb => commandDb.Command.Equals(command));
            this._webhookManager.SendUpdateNotification("/commands");
            return NoContent();
        }

        private bool IsAuthorized() => HttpContext.MeetsAuthorizationLevel(EAuthorizationLevel.MODERATOR);
    }
}
