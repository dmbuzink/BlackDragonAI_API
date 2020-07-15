using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlackDragonAIAPI.Models
{
    public class CommandDetails
    {
//        [BsonElement("command")]
        [Key]
        public string Command { get; set; }

//        [BsonElement("message")]
        public string Message { get; set; }

//        [BsonElement("originalCommand")]
        public string OriginalCommand { get; set; }

//        [BsonElement("permission")]
        public EPermission Permission { get; set; } = EPermission.EVERYONE;

//        [BsonElement("timer")]
        public int Timer { get; set; } = 60;
    }

    public static class CommandDetailsExtensions
    {
        public static Regex GetCommandRegex() => new Regex("![^ \\n]+");
    }
}
