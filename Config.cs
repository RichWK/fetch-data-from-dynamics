using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;

namespace MSAL
{
    public class Config
    {
        public static Dictionary<string, string> GetConnectionValues() => GetValues("Connection");
        public static Dictionary<string, string> GetParameters() => GetValues("Query");
        
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