using System.Collections.Generic;
using System.Threading.Tasks;
using SAFE.Data;
using SAFE.Data.Client;
using SAFE.Data.Client.Auth;
using SafeApp;
using SafeApp.MockAuthBindings;
using SafeApp.Utilities;
using static SAFE.Data.Client.Auth.AuthHelpers;

namespace SAFE.MockAuthClient
{
    public class CredentialAuth : AuthBase
    {
        public CredentialAuth(string appId, bool inMem = true)
            : this(new AppInfo { Id = appId, Name = GetRandomString(10), Vendor = GetRandomString(10) }, inMem)
        { }

        public CredentialAuth(AppInfo appInfo, bool inMem = true)
            : base(appInfo, inMem)
        { }

        public Task<Result<Session>> AuthenticateAsync(Credentials credentials = null)
        {
            credentials ??= new Credentials(GetRandomString(10), GetRandomString(10));

            var authReq = new AuthReq
            {
                App = new AppExchangeInfo { Id = _appInfo.Id, Name = _appInfo.Name, Scope = _appInfo.Scope, Vendor = _appInfo.Vendor },
                AppContainer = true,
                Containers = new List<ContainerPermissions>()
            };

            return AuthenticateAsync(credentials.Locator, credentials.Secret, authReq);
        }

        public Task<Result<Session>> AuthenticateAsync(AuthReq authReq)
        {
            var locator = GetRandomString(10);
            var secret = GetRandomString(10);
            return AuthenticateAsync(locator, secret, authReq);
        }

        public async Task<Result<Session>> AuthenticateAsync(string locator, string secret, AuthReq authReq)
        {
            await ConfigureSession();

            Authenticator authenticator;

            try
            {
                authenticator = await Authenticator.CreateAccountAsync(locator, secret, GetRandomString(5));
            }
            catch
            {
                authenticator = await Authenticator.LoginAsync(locator, secret);
            }

            var (_, reqMsg) = await Session.EncodeAuthReqAsync(authReq);
            var ipcReq = await authenticator.DecodeIpcMessageAsync(reqMsg);
            if (!(ipcReq is AuthIpcReq authIpcReq))
                return new InvalidOperation<Session>($"Could not get {nameof(AuthIpcReq)}");

            var resMsg = await authenticator.EncodeAuthRespAsync(authIpcReq, true);
            var ipcResponse = await Session.DecodeIpcMessageAsync(resMsg);
            if (!(ipcResponse is AuthIpcMsg authResponse))
                return new InvalidOperation<Session>($"Could not get {nameof(AuthIpcMsg)}");

            authenticator.Dispose();

            var session = await Session.AppRegisteredAsync(authReq.App.Id, authResponse.AuthGranted);
            return Result.OK(session);
        }
    }
}