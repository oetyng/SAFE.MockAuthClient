using System;
using System.Linq;

namespace SAFE.MockAuthClient
{
    internal static class AuthHelpers
    {
        // Used to generate random string of any length
        public static string GetRandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}