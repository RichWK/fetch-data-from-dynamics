using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MSAL
{
    // TODO — Change Config class to have multiple methods, one for each "category". This will be neater going forward.
    class Program
    {        
        private static Dictionary<string, string> _config { get; } = Config.GetValues();
        private static IConfidentialClientApplication _app;
        private static string[] _scopes { get; } = new string[] { _config["Scope"] };
        private static AuthenticationResult _authResult = null;

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

                string response = client.GetAsync("https://rebgv.api.crm3.dynamics.com/api/data/v9.1/contacts?$select=spark_contactnumber,firstname,lastname,createdon&$filter=createdon gt 2020-01-25&$orderby=createdon desc")
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;

                Console.WriteLine(response);
            }
        }

        private async static Task<bool> AuthenticateAsync()
        {
            _app = ConfidentialClientApplicationBuilder.Create(_config["ClientId"])
                .WithClientSecret(_config["ClientSecret"])
                .WithAuthority(String.Format(_config["Instance"], _config["Tenant"]))
                .Build();

            _authResult = await _app.AcquireTokenForClient(_scopes).ExecuteAsync();

            return true;
        }
    }
}
