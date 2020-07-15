using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackDragonAIAPI.Models
{
    public class AuthToken
    {
        public AuthToken()
        {

        }

        public AuthToken(string token)
        {
            this.Token = token;
        }

        public string Token { get; set; }
        public EAuthorizationLevel AuthorizationLevel { get; set; }
    }
}
