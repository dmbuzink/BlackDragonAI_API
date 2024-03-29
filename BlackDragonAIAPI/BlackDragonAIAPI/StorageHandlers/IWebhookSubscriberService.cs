using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;

namespace BlackDragonAIAPI.StorageHandlers
{
    public interface IWebhookSubscriberService
    {
        Task<WebhookSubscriber> CreateWebhookSubscriber(WebhookSubscriber webhookSubscriber);
        Task<IEnumerable<WebhookSubscriber>> GetWebhookSubscribers();
    }
}
