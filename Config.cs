using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;

namespace MSAL
{
    public class Config
    {
        public static Dictionary<string, string> GetConnectionValues() => GetValues("Connection");
        public static Dictionary<string, string> GetQueryParameters()
        {
            Dictionary<string, string> queryParameters = GetValues("Query");
            string date = DateTime.Now
                            .AddDays(-Int32.Parse(queryParameters["DaysBack"]))
                            .ToString("yyyy-MM-dd");

            queryParameters.Add("Date", date);

            return queryParameters;
        }
        
        private static Dictionary<string, string> GetValues(string category)
        {
            IEnumerable<IConfigurationSection> config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build()
                .GetSection(category)
                .GetChildren();

            var values = new Dictionary<string, string>();

            foreach(IConfigurationSection entry in config){

                values[entry.Key] = entry.Value;
            }

            return values;
        }
    }
}
