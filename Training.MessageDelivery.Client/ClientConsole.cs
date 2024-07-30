using Training.MessageDelivery.Common.Models;

namespace Training.MessageDelivery.Client
{
    public class ClientConsole
    {
        private readonly IHttpClient _httpClient;
        private readonly IAuthentication _authentication;
        private string _token;
        private string _subscription;
        private List<string> _availableSubscriptions;

        public ClientConsole(IHttpClient httpClient, IAuthentication authentication)
        {
            _httpClient = httpClient;
            _authentication = authentication;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Welcome to the Message Delivery Client!");

            await AuthenticateAsync();

            await SelectSubscriptionAsync();

            await MainLoopAsync();
        }

        private async Task AuthenticateAsync()
        {
            bool authenticated = false;

            while (!authenticated)
            {
                Console.WriteLine("\nAuthenticating...");

                _token = await _authentication.Login();

                if (string.IsNullOrEmpty(_token))
                {
                    Console.WriteLine("Authentication failed.");

                    Console.Write("Retry? (Y/N): ");
                    var retry = Console.ReadLine()?.Trim().ToUpper();

                    if (retry == "N")
                    {
                        Console.WriteLine("Closing application...");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("Authenticated successfully.");
                    authenticated = true;
                }
            }
        }

        private async Task SelectSubscriptionAsync()
        {
            Console.WriteLine("\nFetching available subscriptions...");
            await GetSubscriptionsAsync();

            bool validSubscription = false;

            while (!validSubscription)
            {
                Console.Write("Please select a subscription: ");
                _subscription = Console.ReadLine()?.Trim();

                if (_availableSubscriptions.Contains(_subscription))
                {
                    validSubscription = true;
                    Console.WriteLine($"Subscription '{_subscription}' selected.");
                }
                else
                {
                    Console.WriteLine("Invalid subscription. Please try again.");
                }
            }
        }

        private async Task MainLoopAsync()
        {
            bool exit = false;

            while (!exit)
            {
                ShowMenu();

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        await SendMessageAsync();
                        break;
                    case "2":
                        await ShowMessagesOnHistoryAsync();
                        break;
                    case "3":
                        exit = true;
                        Console.WriteLine("Exiting the application. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void ShowMenu()
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Send a message");
            Console.WriteLine("2. Read the last messages");
            Console.WriteLine("3. Close the application");
            Console.Write("Please choose an option: ");
        }

        private async Task SendMessageAsync()
        {
            Console.Write("Enter your message: ");
            string message = Console.ReadLine();

            try
            {
                await _httpClient.SendMessage(_token, message);
                Console.WriteLine("Message sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message: {ex.Message}");
            }
        }

        private async Task ShowMessagesOnHistoryAsync()
        {
            try
            {
                var response = await _httpClient.MessagesOnHistory(_token, _subscription);
                Console.WriteLine("\nMessage History:");

                foreach (var message in response.Messages)
                {
                    DisplayMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve message history: {ex.Message}");
            }
        }

        private void DisplayMessage(MessagesOnTopic message)
        {
            Console.WriteLine(new string('*', 50));
            Console.WriteLine($"Date: {message.EnqueuedTime:dd-MM-yyyy} | Time: {message.EnqueuedTime:HH:mm}");
            Console.WriteLine("Message:");
            Console.WriteLine(message.MessageText);
            Console.WriteLine(new string('*', 50) + "\n");
        }

        private async Task GetSubscriptionsAsync()
        {
            try
            {
                var response = await _httpClient.GetSubscriptionsForTopic(_token);
                _availableSubscriptions = response.Subscriptions;

                foreach (var subscription in _availableSubscriptions)
                {
                    Console.WriteLine(subscription);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve subscriptions: {ex.Message}");
            }
        }
    }
}
