using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public abstract class ICommandService
    {
        public abstract Task<CommandDetails> CreateCommand(CommandDetails command);
        public abstract Task<IEnumerable<CommandDetails>> GetCommands();
        public abstract Task<IEnumerable<CommandDetails>> GetCommands(Func<CommandDetails, bool> condition);
        public abstract Task<CommandDetails> GetCommand(Func<CommandDetails, bool> condition);
        public abstract Task UpdateCommands(Func<CommandDetails, bool> condition, Func<CommandDetails, CommandDetails> func);
        public abstract Task DeleteCommands(Func<CommandDetails, bool> condition);
    }
}
