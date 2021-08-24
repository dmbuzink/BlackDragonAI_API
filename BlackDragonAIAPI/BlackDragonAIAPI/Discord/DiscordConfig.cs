using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using Discord;
using Discord.WebSocket;

namespace BlackDragonAIAPI.Discord
{
    public class DiscordConfig
    {
        public string Token { get; set; }
        public ulong BotId { get; set; }
        public ulong GuildId { get; set; }
        public ulong StreamPlanningChannelId { get; set; }
        public ulong GeneralChannelId { get; set; }
        public string UpdateMessage { get; set; }
    }
}