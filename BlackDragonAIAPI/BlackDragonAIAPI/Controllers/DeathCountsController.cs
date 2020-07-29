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
    [Route("api/[controller]")]
    [ApiController]
    public class DeathCountsController : ControllerBase
    {
        private readonly IDeathCountsService _deathCountsService;

        public DeathCountsController(IDeathCountsService deathCountsService)
        {
            this._deathCountsService = deathCountsService;
        }

        [HttpPut("{gameId}")]
        public async Task<ActionResult<DeathCount>> UpdateDeathCount(DeathCount deathCount, string gameId)
        {
            try
            {
                return Ok(await CreateOrUpdateDeathCount(gameId, dc =>
                {
                    dc.Deaths = deathCount.Deaths;
                    return dc;
                }));
            }
            catch (ValidationException e)
            {
                return BadRequest(new BadRequestError(e.Message));
            }
        }

        [HttpPost("{gameId}")]
        public async Task<ActionResult<DeathCount>> IncrementDeathCount(string gameId)
        {
            try
            {
                return Ok(await CreateOrUpdateDeathCount(gameId, dc =>
                {
                    dc.Deaths++;
                    return dc;
                }));
            }
            catch (ValidationException e)
            {
                return BadRequest(new BadRequestError(e.Message));
            }
        }

        [HttpDelete("{gameId}")]
        public async Task<ActionResult<DeathCount>> DecrementDeathCount(string gameId)
        {
            try
            {
                return Ok(await CreateOrUpdateDeathCount(gameId, dc =>
                {
                    dc.Deaths--;
                    return dc;
                }));
            }
            catch (ValidationException e)
            {
                return BadRequest(new BadRequestError(e.Message));
            }
        }

        [HttpGet("{gameId}")]
        public async Task<ActionResult<DeathCount>> GetDeathCount(string gameId) =>
            Ok(await this._deathCountsService.GetDeathCount(gameId));

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeathCount>>> GetAllDeathCounts() =>
            Ok(await this._deathCountsService.GetDeathCounts());

        private async Task<DeathCount> CreateOrUpdateDeathCount(string gameId, Func<DeathCount, DeathCount> updateFunc)
        {
            var dbDeathCount = await this._deathCountsService.GetDeathCount(gameId);
            Func<DeathCount, Task> storeInDatabase;
            if (dbDeathCount is null)
            {
                dbDeathCount = new DeathCount()
                {
                    Deaths = 0,
                    GameId = gameId
                };
                storeInDatabase = this._deathCountsService.AddDeathCount;
            }
            else
            {
                storeInDatabase = this._deathCountsService.UpdateDeathCount;
            }

            var deathCountToUpdateTo = updateFunc(dbDeathCount);
            if (!DeathCountIsValid(deathCountToUpdateTo, out var validationException))
                throw validationException;
            await storeInDatabase(deathCountToUpdateTo);
            return deathCountToUpdateTo;
        }

        private static bool DeathCountIsValid(DeathCount deathCount, out ValidationException validationException)
        {
            validationException = deathCount.Deaths >= 0 ? null : new ValidationException("Death count should not be less than 0");
            return validationException == null;
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}
