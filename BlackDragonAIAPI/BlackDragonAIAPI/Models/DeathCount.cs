using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlackDragonAIAPI.Models
{
    public class DeathCount
    {
        [Key]
        public string GameId { get; set; }
        public int Deaths { get; set; } = 0;
    }
}
