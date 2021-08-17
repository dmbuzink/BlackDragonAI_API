using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public class MySqlTimedMessageService : ITimedMessageService
    {
        private readonly BLBDatabaseContext _db;

        public MySqlTimedMessageService(BLBDatabaseContext db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<TimedMessage>> GetTimedMessages() =>
            await Task.Run(() => this._db.TimedMessages);

        public async Task<IEnumerable<TimedMessage>> GetTimedMessages(Func<TimedMessage, bool> condition) =>
            await Task.Run(() => this._db.TimedMessages.AsEnumerable().Where(condition));

        public async Task<TimedMessage> GetTimedMessage(Func<TimedMessage, bool> condition) =>
            await Task.Run(() => this._db.TimedMessages.AsEnumerable().Where(condition).FirstOrDefault());

        public async Task<TimedMessage> AddTimedMessage(TimedMessage timedMessage)
        {
            await this._db.TimedMessages.AddAsync(timedMessage);
            await this._db.SaveChangesAsync();
            return timedMessage;
        }

        public async Task DeleteTimedMessage(string command)
        {
            this._db.TimedMessages.Remove(this._db.TimedMessages.AsQueryable().First(tm => tm.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase)));
            await this._db.SaveChangesAsync();
        }

        public async Task UpdateTimedMessages(Func<TimedMessage, bool> condition, Func<TimedMessage, TimedMessage> func)
        {
            foreach (var tm in this._db.TimedMessages.AsEnumerable().Where(condition))
            {
                this._db.TimedMessages.Update(func(tm));
            }

            await this._db.SaveChangesAsync();
        }
    }
}
