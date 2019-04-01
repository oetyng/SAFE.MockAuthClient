using System;
using System.Threading.Tasks;
using SAFE.Data.Client.Auth;
using SAFE.MockAuthClient;
using SafeApp;
using SafeApp.Utilities;

namespace SAFE.Data.Client
{
    public class BrowserAuth : AuthBase
    {
        public BrowserAuth(AppInfo appInfo, bool inMem = true)
            : base(appInfo, inMem)
        { }

        public async Task<Session> AuthenticateAsync()
        {
            // Authentication with the SAFE browser
            await AuthenticationWithBrowserAsync();

            var authResponse = InterProcessCom.ReceiveAuthResponse();

            // Create session from response
            return await ProcessAuthenticationResponse(authResponse);
        }

        async Task AuthenticationWithBrowserAsync()
        {
            try
            {
                await ConfigureSession();

                // Generate and send auth request to safe-browser for authentication.
                var encodedReq = await AuthHelpers.GenerateEncodedAppRequestAsync(_appInfo);
                var url = AuthHelpers.UrlFormat(_appInfo, encodedReq.Item2, true);
                var info = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true, // not default in netcore, so needs to be set
                    FileName = url
                };
                System.Diagnostics.Process.Start(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw ex;
            }
        }

        async Task<Session> ProcessAuthenticationResponse(string authResponse)
        {
            try
            {
                // Decode auth response and initialise a new session
                var encodedRequest = AuthHelpers.GetRequestData(authResponse);
                var decodeResult = await Session.DecodeIpcMessageAsync(encodedRequest);
                if (decodeResult.GetType() == typeof(AuthIpcMsg))
                {
                    Console.WriteLine("Auth Reqest Granted from Authenticator");

                    // Create session object
                    if (decodeResult is AuthIpcMsg ipcMsg)
                    {
                        // Initialise a new session
                        var session = await Session.AppRegisteredAsync(_appInfo.Id, ipcMsg.AuthGranted);
                        return session;
                    }
                    else
                    {
                        Console.WriteLine("Invalid AuthIpcMsg");
                        throw new Exception("Invalid AuthIpcMsg.");
                    }
                }
                else
                {
                    Console.WriteLine("Auth Request is not Granted");
                    throw new Exception("Auth Request not granted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw ex;
            }
        }
    }
}