using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSAL
{
    public class Authentication
    {
        private static Dictionary<string, string> _conn { get; } = Program.Config.Microsoft;
        private static IConfidentialClientApplication _app;
        private static AuthenticationResult _authResult;

        // This method uses Microsoft's Authentication Library (MSAL).
        // It uses the values specified in appsettings.json to prove this app's identity.
        
        public async static Task<AuthenticationResult> RequestTokenAsync()
        {
            string[] scopes = new string[] { _conn["Scope"] };
            
            Console.WriteLine("Contacting the Dynamics Web API @ " + _conn["Scope"].Replace(".default",""));

            _app = ConfidentialClientApplicationBuilder.Create(_conn["ClientId"])
                .WithClientSecret(_conn["ClientSecret"])
                .WithAuthority(String.Format(_conn["Instance"], _conn["Tenant"]))
                .Build();

            _authResult = await _app.AcquireTokenForClient(scopes).ExecuteAsync();

            return _authResult;
        }
    }
}
