using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.MessageDelivery.ASB;
using Training.MessageDelivery.Client;
using Training.MessageDelivery.Client.Implementations;

class Program
{
    private static async Task Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Setup Dependency Injection
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<IHttpClient>(provider => new HttpClientClass(configuration["WebAPI:BaseEndpoint"]))
            .AddSingleton<IAuthentication, Authentication>()
            .AddSingleton<ClientConsole>()
            .BuildServiceProvider();

        // Resolve the ClientConsole and start the application
        var clientConsole = serviceProvider.GetService<ClientConsole>();
        await clientConsole.StartAsync();
    }
}
