using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public class MySqlWebhookSubscriberService : IWebhookSubscriberService
    {
        private readonly BLBDatabaseContext _db;

        public MySqlWebhookSubscriberService(BLBDatabaseContext db)
        {
            this._db = db;
        }

        public async Task<WebhookSubscriber> CreateWebhookSubscriber(WebhookSubscriber webhookSubscriber)
        {
            await this._db.AddAsync(webhookSubscriber);
            await this._db.SaveChangesAsync();
            return webhookSubscriber;
        }

        public async Task<IEnumerable<WebhookSubscriber>> GetWebhookSubscribers() =>
            this._db.WebhookSubscribers;
    }
}
