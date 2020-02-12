using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MSAL
{
    class Program
    {        
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
            AuthenticationResult _authResult = Authentication.RequestTokenAsync().Result;
            Dictionary<string, string> _query = Config.GetQueryParameters();

            if (_authResult != null)
            {
                HttpClient client = new HttpClient();
                string response;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);

                response = client.GetAsync(String.Format(_query["Uri"], _query["Date"]))
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;

                Console.WriteLine(response);
            }
        }
    }
}
