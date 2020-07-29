using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public interface IDeathCountsService
    {
        Task<DeathCount> AddDeathCount(DeathCount deathCount);
        Task UpdateDeathCount(DeathCount deathCount);
        Task<DeathCount> GetDeathCount(string gameId);
        Task<IEnumerable<DeathCount>> GetDeathCounts();
    }
}
