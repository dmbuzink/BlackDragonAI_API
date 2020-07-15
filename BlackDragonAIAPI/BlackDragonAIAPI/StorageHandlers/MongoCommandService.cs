using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using MongoDB.Driver;

namespace BlackDragonAIAPI.StorageHandlers
{
    public class MongoCommandService : ICommandService
    {
        private readonly IMongoCollection<CommandDetails> _commandCollection;

        public MongoCommandService(IMongoClient client)
        {
            var db = client.GetDatabase("test");
            this._commandCollection = db.GetCollection<CommandDetails>("commands");
        }

        public override async Task<CommandDetails> CreateCommand(CommandDetails command)
        {
            this._commandCollection.InsertOne(command);
            var commands = await this._commandCollection.FindAsync(commandDb => command.OriginalCommand.Equals(command.OriginalCommand));
            return commands.First();
        }

        public override async Task<IEnumerable<CommandDetails>> GetCommands() => await (await this._commandCollection.FindAsync(c => true)).AsQueryableAsync();
        public override async Task<IEnumerable<CommandDetails>> GetCommands(Func<CommandDetails, bool> condition) =>
            (await this.GetCommands()).Where(condition);

        public override async Task<CommandDetails> GetCommand(Func<CommandDetails, bool> condition) =>
            (await GetCommands(condition)).FirstOrDefault();

        public override async Task UpdateCommands(Func<CommandDetails, bool> condition,
            Func<CommandDetails, CommandDetails> func)
        {
            throw new Exception("Fucking broken");
//            await Task.WhenAll(from command in (await GetCommands()).Where(condition)
//                select this._commandCollection.FindOneAndReplaceAsync(commandDb => commandDb.Id.Equals(command.Id),
//                    func(command)));
        }

        public override async Task DeleteCommands(Func<CommandDetails, bool> condition) => 
            await this._commandCollection.DeleteManyAsync(commandDb => condition(commandDb));
    }
}
