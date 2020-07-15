using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlackDragonAIAPI.Models
{
    public class WebhookSubscriber
    {
        [Key]
        public Guid Guid { get; set; }
        public string Uri { get; set; }
    }
}
