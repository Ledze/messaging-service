using System.Net.Http.Json;
using System.Text;
using Training.MessageDelivery.Common.Models;

namespace Training.MessageDelivery.Client.Implementations
{
    public class HttpClientClass : IHttpClient
    {
        private readonly string _baseEndpoint;
        private readonly HttpClient _httpClient;

        public HttpClientClass(string baseEndpoint)
        {
            _baseEndpoint = baseEndpoint;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseEndpoint);
        }

        public async Task<MessagesOnHistoryModel> MessagesOnHistory(string token, string subscription)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_baseEndpoint}/message/messages-on-history/{subscription}"))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<MessagesOnHistoryModel>();
                }
                else
                {
                    throw new Exception("Failed to retrieve messages");
                }
            }
        }

        public async Task<SubscriptionsForTopicModel> GetSubscriptionsForTopic(string token)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_baseEndpoint}/message/subscriptions"))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SubscriptionsForTopicModel>();
                }
                else
                {
                    throw new Exception("Failed to retrieve messages");
                }
            }
        }

        public async Task<bool> SendMessage(string token, string message)
        {
            var content = new StringContent($"\"{message}\"", Encoding.UTF8, "application/json");

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseEndpoint}/message/send-message"))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                requestMessage.Content = content;

                var response = await _httpClient.SendAsync(requestMessage);

                return response.IsSuccessStatusCode;
            }
        }
    }
}
