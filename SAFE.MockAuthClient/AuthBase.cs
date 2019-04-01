using SAFE.Data.Client;
using SafeApp;
using System;
using System.Threading.Tasks;

namespace SAFE.MockAuthClient
{
    public abstract class AuthBase
    {
        const string MOCK_INMEM_KEY = "SAFE_MOCK_IN_MEMORY_STORAGE";
        const string MOCK_UNLIMITED_MUTS_KEY = "SAFE_MOCK_UNLIMITED_MUTATIONS";
        // const string MOCK_VAULT_PATH_KEY = "SAFE_MOCK_VAULT_PATH";

        protected readonly AppInfo _appInfo;

        public AuthBase(AppInfo appInfo, bool inMem = true)
        {
            _appInfo = appInfo;
            Environment.SetEnvironmentVariable(MOCK_UNLIMITED_MUTS_KEY, "true", EnvironmentVariableTarget.Process);
            if (inMem) Environment.SetEnvironmentVariable(MOCK_INMEM_KEY, "true", EnvironmentVariableTarget.Process);
            else Environment.SetEnvironmentVariable(MOCK_INMEM_KEY, null, EnvironmentVariableTarget.Process);
        }

        protected async Task ConfigureSession()
        {
            var exeName = await Session.GetExeFileStemAsync();
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = $"{basePath}{exeName}.safe_core.config";
            await Session.SetAdditionalSearchPathAsync(configPath);
        }
    }
}