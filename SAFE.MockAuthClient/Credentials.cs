using static SAFE.MockAuthClient.AuthHelpers;

namespace SAFE.MockAuthClient
{
    public class Credentials
    {
        public Credentials(string locator, string secret)
        {
            Locator = locator;
            Secret = secret;
        }

        public static Credentials Random => new Credentials(GetRandomString(10), GetRandomString(10));

        public string Locator { get; }
        public string Secret { get; }
    }
}