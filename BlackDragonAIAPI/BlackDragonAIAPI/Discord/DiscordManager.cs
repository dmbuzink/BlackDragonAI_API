using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace BlackDragonAIAPI.Discord
{
    public class DiscordManager : IDiscordManager
    {
        private const string MessagePreText = "bkdnPOG  Jaarplanning  bkdnLurk \r\n\r\n\r\n";
        private readonly DiscordSocketClient _client;

        private readonly DiscordConfig _discordConfig;
        
        public DiscordManager(IOptions<DiscordConfig> discordConfig)
        {
            this._discordConfig = discordConfig.Value;
            this._client = new DiscordSocketClient();
        }

        public async Task Connect()
        {
            ManualResetEventSlim mre = new ManualResetEventSlim(false);
            this._client.Ready += () =>
            {
                mre.Set();
                return Task.CompletedTask;
            };
            await this._client.LoginAsync(TokenType.Bot, this._discordConfig.Token);
            await this._client.StartAsync();
            mre.Wait();
        }

        public async Task<IEnumerable<StreamPlanning>> ReadStreamPlanning()
        {
            var message = await GetMessage();
            var messageText = message.Content.Substring(message.Content.IndexOf("\n",
                StringComparison.InvariantCultureIgnoreCase) + 2);
            return StreamPlanning.MultiParse(messageText);
        }

        public async Task WriteStreamPlanning(IEnumerable<StreamPlanning> streamPlannings)
        {
            var serializedStreamPlanning = StreamPlanning.SerializeMultiple(streamPlannings);
            var streamPlanningText = serializedStreamPlanning.Insert(0, MessagePreText);
            var channel = await GetChannel();
            await channel.ModifyMessageAsync(this._discordConfig.MessageId, mesg =>
                mesg.Content = streamPlanningText);
        }

        public async Task<SocketTextChannel> GetChannel() =>
            await Task.Run(() => this._client
                .GetGuild(this._discordConfig.GuildId)
                .GetTextChannel(this._discordConfig.ChannelId));

        public async Task<IMessage> GetMessage() =>
            await (await this.GetChannel()).GetMessageAsync(this._discordConfig.MessageId);

        public async Task SetUp()
        {
            var channel = await GetChannel();
            await channel.SendMessageAsync("Message that will be used for the planning");
        }
    }
}