using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public interface ITimedMessageService
    {
        Task<IEnumerable<TimedMessage>> GetTimedMessages();
        Task<IEnumerable<TimedMessage>> GetTimedMessages(Func<TimedMessage, bool> condition);
        Task<TimedMessage> GetTimedMessage(Func<TimedMessage, bool> condition);
        Task<TimedMessage> AddTimedMessage(TimedMessage timedMessage);
        Task DeleteTimedMessage(string command);
        Task UpdateTimedMessages(Func<TimedMessage, bool> condition, Func<TimedMessage, TimedMessage> func);

    }
}
