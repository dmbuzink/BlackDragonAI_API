using System.ComponentModel.DataAnnotations;

namespace BlackDragonAIAPI.Models
{
    public class DeathCount
    {
        [Key]
        public string GameId { get; set; }
        public int Deaths { get; set; } = 0;
        public bool IsDeathCount { get; set; } = true;
    }
}
