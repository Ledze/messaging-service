using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Text;
using Training.MessageDelivery.Common.Models;

namespace Training.MessageDelivery.ASB
{
    public class MessageService : IMessageService
    {

        private readonly string _serviceBusConnectionString;
        private readonly string _topicName;

        public MessageService(string serviceBusConnectionString, string topicName)
        {
            _serviceBusConnectionString = serviceBusConnectionString ?? throw new ArgumentNullException(nameof(serviceBusConnectionString));
            _topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
        }

        public async Task SendMessageAsync(string serviceBusMessage)
        {
            try
            {
                await using var client = new ServiceBusClient(_serviceBusConnectionString);

                ServiceBusSender sender = client.CreateSender(_topicName);

                ServiceBusMessage message = new ServiceBusMessage(Encoding.UTF8.GetBytes(serviceBusMessage));

                await sender.SendMessageAsync(message);
                await sender.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<MessagesOnHistoryModel> PeekTopicMessagesAsync(string subscriptionName)
        {
            MessagesOnHistoryModel messages = new MessagesOnHistoryModel();
            messages.Messages = new List<MessagesOnTopic>();

            try
            {
                await using var client = new ServiceBusClient(_serviceBusConnectionString);
                ServiceBusReceiver receiver = client.CreateReceiver(_topicName, subscriptionName);

                long sequenceNumber = 0;

                while (true)
                {
                    ServiceBusReceivedMessage receivedMessage = await receiver.PeekMessageAsync(sequenceNumber);

                    if (receivedMessage == null)
                    {
                        break; // No more messages to peek
                    }

                    string body = Encoding.UTF8.GetString(receivedMessage.Body);
                    messages.Messages.Add(new MessagesOnTopic()
                    {
                        MessageId = receivedMessage.MessageId,
                        EnqueuedTime = receivedMessage.EnqueuedTime.DateTime,
                        MessageText = body
                    });

                    sequenceNumber = receivedMessage.SequenceNumber + 1;
                }

                await receiver.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }

            return messages;
        }

        public async Task<SubscriptionsForTopicModel> GetSubscriptionsForTopicAsync()
        {
            var subscriptionsList = new SubscriptionsForTopicModel();
            subscriptionsList.Subscriptions = new List<string>();

            try
            {
                var adminClient = new ServiceBusAdministrationClient(_serviceBusConnectionString);

                var subscriptions = adminClient.GetSubscriptionsAsync(_topicName);
                
                await foreach (var subscription in subscriptions)
                {
                    subscriptionsList.Subscriptions.Add(subscription.SubscriptionName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return subscriptionsList;
        }
    }
}
