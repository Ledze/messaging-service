using Azure.Messaging.ServiceBus;
using Training.MessageDelivery.Common.Models;

namespace Training.MessageDelivery.ASB
{
    public interface IMessageService
    {
        Task SendMessageAsync(string serviceBusMessage);

        Task<MessagesOnHistoryModel> PeekTopicMessagesAsync(string subscriptionName);

        Task<SubscriptionsForTopicModel> GetSubscriptionsForTopicAsync();
    }
}
