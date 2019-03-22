using System;
using Microsoft.Win32;
using SafeApp.Utilities;

namespace SAFE.Data.Client.Auth
{
    public class AuthConfig
    {
        public static void WinDesktopUseBrowserAuth(AppInfo appInfo, string appPath)
            => BrowserAuthHelpers.RegisterAppProtocol(appInfo, appPath);
    }

    internal static class BrowserAuthHelpers
    {
        // Registering URL Protocol in System Registry using full path of the application
        public static void RegisterAppProtocol(AppInfo appInfo, string appPath)
        {
            Console.WriteLine("\nRegistering Apps URL Protocol in Registry");

            // open App's protocol's subkey
            RegistryKey mainKey = Registry.CurrentUser.OpenSubKey("Software", true)?.OpenSubKey("Classes", true);

            char[] padding = { '=' };
            string appUrl = "safe-" + Convert.ToBase64String(appInfo.Id.ToUtfBytes().ToArray())
                .TrimEnd(padding).Replace('+', '-').Replace('/', '_');

            var key = mainKey?.OpenSubKey(appUrl, true);

            // because two apps are using same registry key so
            // we are deleting the already present registry key, and then adding a new one
            if (key != null)
            {
                mainKey.DeleteSubKeyTree(appUrl);
                key = mainKey.OpenSubKey(appUrl, true);
            }

            if (appPath.EndsWith(".dll"))
                appPath = appPath.Replace(".dll", ".exe");

            // if the protocol is not registered yet...we register it
            if (key == null)
            {
                key = mainKey.CreateSubKey(appUrl);
                key.SetValue(string.Empty, "URL: dotUrlRegister Protocol");
                key.SetValue("URL Protocol", string.Empty);

                // %1 represents the argument - this tells windows to open this program with an argument / parameter
                key = key.CreateSubKey(@"shell\open\command");
                key.SetValue(string.Empty, appPath + " " + "%1");
            }
            key.Close();
        }
    }
}