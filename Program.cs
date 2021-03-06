﻿using Microsoft.Identity.Client;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FetchDataFromDynamics
{
    class Program
    {
        public static Configuration Config { get; } = new Configuration();
        public static List<Contact> Contacts { get; private set; }
        public static string Json { get; private set; }
        private static int _wait { get; } = 20;

        static void Main()
        {
            try
            {
                FetchData();
                CopyDataToSQLServer();
                Logging.WriteLogFile();
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex);
            }

            Console.WriteLine($"Exiting in {_wait} seconds...");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(_wait));
        }


        private static void FetchData()
        {
            AuthenticationResult _authResult = Authentication.RequestTokenAsync().Result;

            HttpClient client = new HttpClient();
            string response;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer"
                ,_authResult.AccessToken
            );

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

            foreach (JToken item in jsonSubset)
            {
                Contact contact = item.ToObject<Contact>();
                contacts.Add(contact);
            }

            jsonOutput = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            Console.WriteLine($"Fetched {contacts.Count} recently created members.");

            Contacts = contacts;
            Json = jsonOutput;
        }


        private static string SanitizeInput(string json)
        {
            return json
                .Replace(";", "")
                .Replace("--", "")
                .Replace("/*", "")
                .Replace("*/", "")
                .Replace("xp_", "");
        }


        private static void CopyDataToSQLServer()
        {
            if (Contacts.Count() > 0)
            {
                DataTable table = new ContactTable();
                DataRow row;

                foreach (Contact contact in Contacts)
                {
                    row = table.NewRow();
                    row["CUST_NO"] = contact.Spark_ContactNumber;
                    row["CARDYN"] = contact.CardCreated;
                    row["NUM_CARDS"] = contact.NumberOfCards;
                    row["IMAGE_PATH"] = contact.ImagePath;
                    table.Rows.Add(row);
                }

                BulkCopyAsync(table).Wait();
            }
        }


        private async static Task BulkCopyAsync(DataTable table)
        {
            int initialRowCount;
            int finalRowCount;

            Console.WriteLine(
                $"Attempting to copy data into the '{Config.SQLServer["Table"]}' table..."
            );

            using SqlConnection conn = new SqlConnection(
                Config.SQLServer["DevConnectionString"]
            );
            
            await conn.OpenAsync();

            initialRowCount = await CountRows(conn);
            Console.WriteLine($"The table currently holds {initialRowCount} rows.");

            using SqlBulkCopy bulkCopy = new SqlBulkCopy(conn)
            {
                DestinationTableName = Config.SQLServer["Table"]
            };

            bulkCopy.WriteToServerAsync(table).Wait();

            finalRowCount = await CountRows(conn);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
                $"Successfully added {finalRowCount - initialRowCount} rows. (Any ignored members already exist.)"
            );

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"The table now holds {finalRowCount} rows.");
        }


        private async static Task<int> CountRows(SqlConnection conn)
        {
            SqlCommand count = new SqlCommand(
                $"select count(CUST_NO) from {Config.SQLServer["Table"]}"
                ,conn
            );

            return System.Convert.ToInt32(await count.ExecuteScalarAsync());
        }
    }
}

