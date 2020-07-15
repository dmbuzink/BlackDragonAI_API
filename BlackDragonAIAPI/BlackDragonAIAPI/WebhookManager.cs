using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BlackDragonAIAPI.StorageHandlers;

namespace BlackDragonAIAPI
{
    public class WebhookManager
    {
        private IWebhookSubscriberService _db;
        private static readonly HttpClient _client = new HttpClient();

        public WebhookManager(IWebhookSubscriberService db)
        {
            this._db = db;
        }

        public async void SendUpdateNotification(string endpoint)
        {
            Console.WriteLine("Starting");
            foreach (var ws in await this._db.GetWebhookSubscribers())
            {
                try
                {
                    var url = ws.Uri + endpoint;
                    Console.WriteLine($"URL: {url}");
                    SendWithoutWaiting(new Uri(url));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Webhook error to uri: {ws.Uri}");
                }
            }
        }

        private async void SendWithoutWaiting(Uri uri)
        {
            try
            {
                await _client.PostAsync(uri, new StringContent(""));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Webhook error to uri: {uri.AbsolutePath}");
            }
        }
    }
}
