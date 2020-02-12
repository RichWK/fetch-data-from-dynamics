using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSAL
{
    public class Authentication
    {
        private static IConfidentialClientApplication _app;
        private static AuthenticationResult _authResult;
        private static Dictionary<string, string> _connection { get; } = Config.GetConnectionValues();
        private static string[] _scopes { get; } = new string[] { _connection["Scope"] };

        // This method makes use of Microsoft's Authentication Library (MSAL).
        // It uses the values in appsettings.json to prove this app's identity.
        
        public async static Task<AuthenticationResult> RequestTokenAsync()
        {
            _app = ConfidentialClientApplicationBuilder.Create(_connection["ClientId"])
                .WithClientSecret(_connection["ClientSecret"])
                .WithAuthority(String.Format(_connection["Instance"], _connection["Tenant"]))
                .Build();

            _authResult = await _app.AcquireTokenForClient(_scopes).ExecuteAsync();

            return _authResult;
        }
    }
}
