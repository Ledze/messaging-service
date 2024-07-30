using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.MessageDelivery.Client.Implementations
{
    public class Authentication : IAuthentication
    {
        private string _clientId;
        private string _tenantId;
        private string[] _scopes;
        private string _authority;
        private string _redirectUri = @"http://localhost";

        private readonly IConfiguration _configuration;

        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;

            _clientId = _configuration["AzureAD:ClientId"];
            _tenantId = _configuration["AzureAD:TenantId"];
            _scopes = new[] { _configuration["AzureAD:Scope"] };
            _authority = _configuration["AzureAD:Authority"];
        }

        public async Task<string> Login()
        {
            IPublicClientApplication app = PublicClientApplicationBuilder.Create(_clientId)
                .WithRedirectUri(_redirectUri)
                .WithAuthority(_authority)
                .Build();

            AuthenticationResult result = null;

            try
            {
                var accounts = await app.GetAccountsAsync();
                result = await app.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();

                Console.WriteLine("Token acquired silently.");
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    result = await app.AcquireTokenInteractive(_scopes)
                                      .WithPrompt(Prompt.SelectAccount)
                                      .ExecuteAsync();

                    Console.WriteLine("Token acquired interactively.");

                }
                catch (MsalException ex)
                {
                    Console.WriteLine($"An error occurred acquiring a token interactively: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred acquiring a token silently: {ex.Message}");
            }

            return result?.AccessToken;
        }
    }
}
