using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public interface IUserService
    {
        Task<User> GetUserByName(string username);
        Task<User> AddUser(User user);
        Task UpdateAuthLevel(string username, EAuthorizationLevel authLevel);
        Task<IEnumerable<User>> GetUsers(Func<User, bool> condition);
    }
}
