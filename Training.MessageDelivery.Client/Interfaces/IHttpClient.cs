using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.MessageDelivery.Common.Models;

namespace Training.MessageDelivery.Client
{
    public interface IHttpClient
    {
        Task<MessagesOnHistoryModel> MessagesOnHistory(string token, string subscription);

        Task<bool> SendMessage(string token, string message);

        Task<SubscriptionsForTopicModel> GetSubscriptionsForTopic(string token);
    }
}
