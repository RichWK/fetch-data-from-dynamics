using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;

namespace FetchDataFromDynamics
{
    public class Configuration
    {
        public Dictionary<string, string> Microsoft { get; } = GetValues("Microsoft");
        public Dictionary<string, string> Query { get; } = GetValues("Query");
        public Dictionary<string, string> SQLServer { get; } = GetValues("SQLServer");
        
        public Configuration()
        {
            string date = DateTime
                .Now
                .AddDays(-Int32.Parse(Query["DaysBack"]))
                .ToString("yyyy-MM-dd");

            Query.Add("Date", date);
        }
        
        private static Dictionary<string, string> GetValues(string category)
        {
            IEnumerable<IConfigurationSection> config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
