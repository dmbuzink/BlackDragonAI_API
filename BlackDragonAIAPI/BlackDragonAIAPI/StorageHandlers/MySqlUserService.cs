using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlackDragonAIAPI.StorageHandlers
{
    public class MySqlUserService : IUserService
    {
        private readonly BLBDatabaseContext _db;

        public MySqlUserService(BLBDatabaseContext db)
        {
            this._db = db;
        }

        public async Task<User> GetUserByName(string username) => 
            await this._db.Users.FirstOrDefaultAsync(u => 
                u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));

        public async Task<User> AddUser(User user)
        {
            await this._db.Users.AddAsync(user);
            await this._db.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAuthLevel(string username, EAuthorizationLevel authLevel)
        {
            var user = await GetUserByName(username);
            user.AuthorizationLevel = authLevel;
            this._db.Users.Update(user);
            await this._db.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsers(Func<User, bool> condition) =>
            this._db.Users.Where(condition);
    }
}
