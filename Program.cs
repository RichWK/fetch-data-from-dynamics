using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MSAL
{
    class Program
    {        
        private static List<Contact> _contacts;
        private static string _json;

        static void Main()
        {
            try
            {
                (_contacts, _json) = FetchData();

                Console.WriteLine(_json);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.Read();
        }


        private static (List<Contact>,string) FetchData()
        {
            AuthenticationResult _authResult = Authentication.RequestTokenAsync().Result;
            Dictionary<string, string> _query = Config.GetQueryParameters();

            HttpClient client = new HttpClient();
            string response;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);

            response = client.GetAsync(String.Format(_query["Uri"], _query["Date"]))
                .Result
                .Content
                .ReadAsStringAsync()
                .Result;

            return MassageData(response);
        }


        private static (List<Contact>,string) MassageData(string json)
        {
            List<Contact> contacts = new List<Contact>();
            JObject jsonObject = JObject.Parse(SanitizeInput(json));
            List<JToken> jsonSubset = jsonObject["value"].Children().ToList();
            string jsonOutput;

            foreach(JToken item in jsonSubset)
            {
                Contact contact = item.ToObject<Contact>();
                contacts.Add(contact);
            }

            jsonOutput = JsonConvert.SerializeObject(contacts,Formatting.Indented);
            
            return (contacts,jsonOutput);
        }


        private static string SanitizeInput(string json)
        {
            return json.Replace(";", "").Replace("--", "").Replace("/*", "").Replace("*/", "").Replace("xp_", "");
        }
    }
}
