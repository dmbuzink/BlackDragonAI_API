using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public interface IStreamPlanningService
    {
        Task<StreamPlanning> CreateStreamPlanning(StreamPlanning streamPlanning);
        Task<StreamPlanning> UpdateStreamPlanning(StreamPlanning streamPlanning);
        Task<IEnumerable<StreamPlanning>> GetStreamPlannings(Func<StreamPlanning, bool> condition = null);
        Task<StreamPlanning> GetStreamPlanningById(long id);
        Task DeleteStreamPlanningById(long id);
    }
}