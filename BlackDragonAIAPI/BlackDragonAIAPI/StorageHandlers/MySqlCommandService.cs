using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public class MySqlCommandService : ICommandService
    {
        private readonly BLBDatabaseContext _db;

        public MySqlCommandService(BLBDatabaseContext db)
        {
            this._db = db;
        }

        public override async Task<CommandDetails> CreateCommand(CommandDetails command)
        {
            await this._db.Commands.AddAsync(command);
            await this._db.SaveChangesAsync();
            return command;
        }

        public override async Task<IEnumerable<CommandDetails>> GetCommands() =>
            await Task.Run(() => this._db.Commands);

        public override async Task<IEnumerable<CommandDetails>> GetCommands(Func<CommandDetails, bool> condition) =>
            await Task.Run(() => this._db.Commands.AsEnumerable().Where(condition));

        public override async Task<CommandDetails> GetCommand(Func<CommandDetails, bool> condition) =>
            (await GetCommands(condition)).FirstOrDefault();

        public override async Task UpdateCommands(Func<CommandDetails, bool> condition, Func<CommandDetails, CommandDetails> func)
        {
            foreach (var command in this._db.Commands.AsEnumerable().Where(condition))
            {
                this._db.Commands.Update(func(command));
            }

            await this._db.SaveChangesAsync();
        }

        public override async Task DeleteCommands(Func<CommandDetails, bool> condition)
        {
            this._db.Commands.RemoveRange(await this.GetCommands(condition));
            await this._db.SaveChangesAsync();
        }
    }
}
