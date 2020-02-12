using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MSAL
{
    class Program
    {        
        public static Configuration Config { get; } = new Configuration();
        public static List<Contact> Contacts { get; private set; }
        public static string Json { get; private set; }

        static void Main()
        {
            try
            {
                FetchData();
                CopyDataToSQLServer();
                Logging.WriteLogFile();
            }
            catch(Exception ex)
            {
                Logging.HandleException(ex);
            }
            
            Console.Read();
        }

        private static void FetchData()
        {
            AuthenticationResult _authResult = Authentication.RequestTokenAsync().Result;

            HttpClient client = new HttpClient();
            string response;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);

            response = client
                .GetAsync(String.Format(Config.Query["Uri"], Config.Query["Date"]))
                .Result
                .Content
                .ReadAsStringAsync()
                .Result;

            TransformData(response);
        }

        private static void TransformData(string json)
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

            Console.WriteLine("Fetched {0} contacts.", contacts.Count);
            
            Contacts = contacts;
            Json = jsonOutput;
        }

        private static string SanitizeInput(string json)
        {
            return json.Replace(";", "").Replace("--", "").Replace("/*", "").Replace("*/", "").Replace("xp_", "");
        }

        private static void CopyDataToSQLServer()
        {
            if(Contacts.Count() > 0)
            {
                DataTable table = new ContactTable();
                DataRow row;

                foreach(Contact contact in Contacts)
                {
                    row = table.NewRow();
                    row["ContactId"] = contact.Spark_ContactNumber;
                    row["FirstName"] = contact.FirstName;
                    row["LastName"] = contact.LastName;
                    row["CreatedInDynamicsOn"] = contact.CreatedOn;
                    table.Rows.Add(row);
                }

                BulkCopyAsync(table).Wait();
            }
        }

        private async static Task BulkCopyAsync(DataTable table)
        {

        }
    }
}
