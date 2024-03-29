using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackDragonAIAPI.Models
{
    public class Existence
    {
        public Existence(bool exists)
        {
            this.Exists = exists;
        }

        public bool Exists { get; set; }
    }
}
