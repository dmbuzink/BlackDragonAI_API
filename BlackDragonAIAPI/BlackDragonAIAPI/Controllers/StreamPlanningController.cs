using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Discord;
using BlackDragonAIAPI.Models;
using BlackDragonAIAPI.StorageHandlers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BlackDragonAIAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamPlanningController : ControllerBase
    {
        private const string DiscordPreText = "bkdnPOG  Jaarplanning  bkdnLurk\r\n\r\n";
        private readonly IStreamPlanningService _streamPlanningService;
        private readonly IDiscordManager _discordManager;

        public StreamPlanningController(IStreamPlanningService streamPlanningService, IDiscordManager discordManager)
        {
            this._streamPlanningService = streamPlanningService;
            this._discordManager = discordManager;
        }

        [HttpPost]
        public async Task<ActionResult> CreateStreamPlanning(StreamPlanning streamPlanning)
        {
            var createdSp = await this._streamPlanningService.CreateStreamPlanning(streamPlanning);
            return Created($"/api/streamplannings/{streamPlanning.Id}", createdSp);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateStreamPlanning(StreamPlanning streamPlanning)
        {
            var updatedSp = await this._streamPlanningService.UpdateStreamPlanning(streamPlanning);
            return Ok(updatedSp);
        }

        [HttpGet("id")]
        public async Task<ActionResult<StreamPlanning>> GetStreamPlanningById(long id) =>
            Ok(await this._streamPlanningService.GetStreamPlanningById(id));

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StreamPlanning>>> GetStreamPlannings() => 
            Ok(await this._streamPlanningService.GetStreamPlannings());

        [HttpDelete("id")]
        public async Task<ActionResult> DeleteStreamPlanning(long id)
        {
            await this._streamPlanningService.DeleteStreamPlanningById(id);
            return NoContent();
        }

        [HttpPut("discord/update")]
        public async Task<ActionResult> UpdateDiscordPlanning()
        {
            var streamPlannings = await this._streamPlanningService.GetStreamPlannings();
            await this._discordManager.WriteStreamPlanning(streamPlannings);
            return NoContent();
        }

        [HttpPut("discord/load")]
        public async Task<ActionResult> LoadDiscordPlanning()
        {
            var discordStreamPlannings = await this._discordManager.ReadStreamPlanning();
            var dbStreamPlannings = await this._streamPlanningService.GetStreamPlannings();
            foreach (var sp in dbStreamPlannings)
            {
                await this._streamPlanningService.DeleteStreamPlanningById(sp.Id);
            }
            foreach (var sp in discordStreamPlannings)
            {
                await this._streamPlanningService.CreateStreamPlanning(sp);
            }
            return NoContent();
        }
    }
}