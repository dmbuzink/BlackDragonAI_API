#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlackDragonAIAPI.StorageHandlers
{
    public class MysqlStreamPlanningService : IStreamPlanningService
    {
        private readonly BLBDatabaseContext _db;

        public MysqlStreamPlanningService(BLBDatabaseContext db)
        {
            this._db = db;
        }
        
        public async Task<StreamPlanning> CreateStreamPlanning(StreamPlanning streamPlanning)
        {
            await this._db.StreamPlannings.AddAsync(streamPlanning);
            await this._db.SaveChangesAsync();
            return streamPlanning;
        }

        public async Task<StreamPlanning> UpdateStreamPlanning(StreamPlanning streamPlanning)
        {
            var dbSp = await GetStreamPlanningById(streamPlanning.Id);
            if (dbSp is null) throw new ArgumentException("Id is not connected to a stream planning");
            dbSp.Date = streamPlanning.Date;
            dbSp.Game = streamPlanning.Game;
            dbSp.GameType = streamPlanning.GameType;
            dbSp.StreamType = streamPlanning.StreamType;
            dbSp.TimeSlot = streamPlanning.TimeSlot;
            dbSp.TrailerUri = streamPlanning.TrailerUri;
            this._db.StreamPlannings.Update(dbSp);
            await this._db.SaveChangesAsync();
            return streamPlanning;
        }

        public Task<IEnumerable<StreamPlanning>> GetStreamPlannings(Func<StreamPlanning, bool>? condition = null)
        {
            return Task.Run(() => condition is null ? this._db.StreamPlannings : 
                this._db.StreamPlannings.AsEnumerable().Where(condition));
        }

        public async Task<StreamPlanning?> GetStreamPlanningById(long id)
        {
            return await this._db.StreamPlannings.AsQueryable().FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task DeleteStreamPlanningById(long id)
        {
            var sp = await GetStreamPlanningById(id);
            if (sp is null) throw new ArgumentException("Id is not connected to a stream planning");
            this._db.StreamPlannings.Remove(sp);
            await this._db.SaveChangesAsync();
        }
    }
}