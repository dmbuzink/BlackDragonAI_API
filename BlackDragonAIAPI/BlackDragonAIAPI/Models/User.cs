using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlackDragonAIAPI.Models
{
    public class User
    {
//        [BsonElement("_id")]
//        [BsonRepresentation(BsonType.ObjectId)] 
//        public ObjectId Id{ get; set; }

//        [BsonElement("username")] 
        [Key]
        public string Username { get; set; } = "";
//        [BsonElement("password")] 
        public string Password { get; set; } = "";
//        [BsonElement("authorizationLevel")]
        public EAuthorizationLevel AuthorizationLevel { get; set; } = EAuthorizationLevel.NONE;
    }
}
