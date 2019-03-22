using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Utilities;

namespace SAFE.Data.Client.Auth
{
    internal static class AuthHelpers
    {
        // Add safe-auth:// in encoded auth request
        public static string UrlFormat(AppInfo appInfo, string encodedString, bool toAuthenticator)
        {
            var scheme = toAuthenticator ? "safe-auth" : $"{appInfo.Id}";
            return $"{scheme}://{encodedString}";
        }

        public static string GetRequestData(string url)
            => new Uri(url).PathAndQuery.Replace("/", string.Empty);

        // Generating encoded app request using appname, appid, vendor
        public static async Task<(uint, string)> GenerateEncodedAppRequestAsync(AppInfo appInfo)
        {
            Console.WriteLine("\nGenerating application authentication request");

            // Create an AuthReq object
            var authReq = new AuthReq
            {
                AppContainer = true,
                App = new AppExchangeInfo { Id = appInfo.Id, Scope = string.Empty, Name = appInfo.Name, Vendor = appInfo.Vendor },
                Containers = new List<ContainerPermissions>()
            };

            // Return encoded AuthReq
            return await Session.EncodeAuthReqAsync(authReq);
        }

        // Used to generate random string of any length
        public static string GetRandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}