// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using BlackDragonAIAPI.Models;
// using MongoDB.Driver;
//
// namespace BlackDragonAIAPI.StorageHandlers
// {
//     public class MongoUserService : IUserService
//     {
//         private readonly IMongoCollection<User> _userCollection;
//
//         public MongoUserService(IMongoClient client)
//         {
//             var db = client.GetDatabase("test");
//             this._userCollection = db.GetCollection<User>("users");
//         }
//
//         public async Task<User> GetUserByName(string username) =>
//             (await this._userCollection.FindAsync(u => u.Username.Equals(username)))
//             .FirstOrDefault();
//         public async Task<User> AddUser(User user)
//         {
//             await this._userCollection.InsertOneAsync(user);
//             return await GetUserByName(user.Username);
//         }
//
//         public async Task UpdateAuthLevel(string username, EAuthorizationLevel authLevel) =>
//             await this._userCollection.UpdateOneAsync(u => u.Username.Equals(username), 
//                 new UpdateDefinitionBuilder<User>().Set("authorizationLevel", authLevel));
//
//         public Task<IEnumerable<User>> GetUsers(Func<User, bool> condition)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }
