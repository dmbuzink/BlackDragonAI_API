using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.Discord
{
    public interface IDiscordManager
    {
        Task Connect();
        Task<IEnumerable<StreamPlanning>> ReadStreamPlanning();
        Task WriteStreamPlanning(IEnumerable<StreamPlanning> streamPlannings);
    }
}