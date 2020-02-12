using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MSAL
{
    class Program
    {        
        private static IConfidentialClientApplication _app;
        private static AuthenticationResult _authResult = null;
        private static Dictionary<string, string> _connection { get; } = Config.GetConnectionValues();
        private static Dictionary<string, string> _query { get; } = Config.GetParameters();
        private static string[] _scopes { get; } = new string[] { _connection["Scope"] };

        static void Main()
        {
            try
            {
                FetchData();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.Read();
        }

        private static void FetchData()
        {
            if (AuthenticateAsync().Result)
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);

                string response = client.GetAsync(_query["Uri"])
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;

                Console.WriteLine(response);
            }
        }

        private async static Task<bool> AuthenticateAsync()
        {
            _app = ConfidentialClientApplicationBuilder.Create(_connection["ClientId"])
                .WithClientSecret(_connection["ClientSecret"])
                .WithAuthority(String.Format(_connection["Instance"], _connection["Tenant"]))
                .Build();

            _authResult = await _app.AcquireTokenForClient(_scopes).ExecuteAsync();

            return true;
        }
    }
}
