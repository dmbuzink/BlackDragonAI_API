using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private const string MessagePreText = "**Jaarplanning**  \r\n\r\n\r\n";
        private const ushort DiscordCharacterLimit = 2000;
        private readonly DiscordSocketClient _client;
        private readonly DiscordConfig _discordConfig;
        private bool _isConnected = false;
        
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
            this._isConnected = true;
        }

        public async Task<IEnumerable<StreamPlanning>> ReadStreamPlannings()
        {
            if (!_isConnected) await Connect();

            var messages = (await GetStreamPlanningMessages())
                .Where(mesg => !mesg.Content.Equals("_ _"))
                .Select(mesg => mesg.Content);
            var mergedTextMessage = string.Join("\r\n\r\n", messages)
                .Replace("\r", string.Empty);
            
            mergedTextMessage = mergedTextMessage
                .Substring(mergedTextMessage.IndexOf("\n\n\n",
                    StringComparison.InvariantCultureIgnoreCase) + 6);
            return StreamPlanning.MultiParse(mergedTextMessage);



            // var message = await GetMessage();
            // var messageText = message.Content.Replace("\r", string.Empty);
            // messageText = messageText
            //     .Substring(messageText.IndexOf("\n\n\n",
            //         StringComparison.InvariantCultureIgnoreCase) + 6);

            // return StreamPlanning.MultiParse(messageText);
        }

        public async Task WriteStreamPlanning(IEnumerable<StreamPlanning> streamPlannings)
        {
            if (!_isConnected) await Connect();
            
            // var serializedStreamPlanning = StreamPlanning.SerializeMultiple(streamPlannings);
            // var streamPlanningText = serializedStreamPlanning.Insert(0, MessagePreText);

            var serializedStreamPlannings = SerializeStreamPlanningIntoMessages(streamPlannings);
            var channel = await GetStreamPlanningChannel();
            var spMessages = await GetStreamPlanningMessages();
            await channel.DeleteMessagesAsync(spMessages);
            foreach (var spMessage in serializedStreamPlannings)
            {
                await channel.SendMessageAsync(spMessage);
            }
        }

        private async Task<IEnumerable<IMessage>> GetStreamPlanningMessages() =>
            (await (await GetStreamPlanningChannel()).GetMessagesAsync().FlattenAsync())
                .Where(mesg => mesg.Author.Id == this._discordConfig.BotId)
                .OrderBy(mesg => mesg.CreatedAt)
            ;

        private static IEnumerable<string> SerializeStreamPlanningIntoMessages(IEnumerable<StreamPlanning> streamPlannings)
        {
            var serializedStreamPlannings = streamPlannings.OrderBy(sp => sp.Date).Select(sp => sp.ToString()).ToList();
            var sb = new StringBuilder(MessagePreText);
            var finalizedMessages = new List<string>();
            for(var i = 0; i < serializedStreamPlannings.Count(); i++)
            {
                var serializedSp = serializedStreamPlannings.ElementAt(i);
                if (i != serializedStreamPlannings.Count() - 1)
                {
                    serializedSp = $"{serializedSp}\r\n\r\n";
                }
                
                if (sb.Length + serializedSp.Length <= DiscordCharacterLimit)
                {
                    sb.Append(serializedSp);
                }
                else
                {
                    finalizedMessages.Add(sb.ToString());
                    sb = new StringBuilder();
                    sb.Append(serializedSp);
                }
            }

            if (sb.Length > 0)
            {
                finalizedMessages.Add(sb.ToString());
            }

            var resultList = new List<string>();
            for (var i = 0; i < finalizedMessages.Count; i++)
            {
                resultList.Add(finalizedMessages.ElementAt(i));
                if (i != finalizedMessages.Count() - 1)
                {
                    resultList.Add("_ _");
                }
            }
            return resultList;
        }

        public async Task<IMessage> ReadMessageAsync(ulong messageId)
        {
            var channel = await GetStreamPlanningChannel();
            return await channel.GetMessageAsync(messageId);
        }

        public async Task ShareUpdatedMessage()
        {
            var channel = await GetGeneralChannel();
            await channel.SendMessageAsync(this._discordConfig.UpdateMessage);
        }

        public async Task SendMessage(ulong channelId, string text)
        {
            var channel = (await GetGuild()).GetTextChannel(channelId);
            await channel.SendMessageAsync(text);
        }

        private async Task<SocketTextChannel> GetGeneralChannel() =>
            (await GetGuild()).GetTextChannel(this._discordConfig.GeneralChannelId);

        public async Task<SocketTextChannel> GetStreamPlanningChannel() =>
            (await GetGuild()).GetTextChannel(this._discordConfig.StreamPlanningChannelId);

        private async Task<SocketGuild> GetGuild() =>
            await Task.Run(() => this._client.GetGuild(this._discordConfig.GuildId));

        public async Task SetUp()
        {
            var channel = await GetStreamPlanningChannel();
            await channel.SendMessageAsync("Message that will be used for the planning");
        }
    }
}