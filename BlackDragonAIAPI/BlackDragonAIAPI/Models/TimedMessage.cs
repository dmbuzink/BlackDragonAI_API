using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlackDragonAIAPI.Models
{
    public class TimedMessage
    {
        [Key] public Guid Guid { get; set; } = Guid.NewGuid();
        public string Command { get; set; }
        public int IntervalInMinutes { get; set; } = 30;
        public int OffsetInMinutes { get; set; } = 0;
    }
}
