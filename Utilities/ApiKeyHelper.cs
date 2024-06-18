using System.Security.Cryptography;
using System.Text;

namespace ResuMeAPI.Utilities
{
    public static class ApiKeyHelper
    {
        private const string _prefix = "RM-";
        private const int _numberOfSecureBytesToGenerate = 32;
        private const int _lengthOfKey = 32;

        public static string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);

            string base64String = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_");

            var keyLength = _lengthOfKey - _prefix.Length;

            return _prefix + base64String[..keyLength];
        }

        public static string HashApiKey(string apiKey)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(apiKey);
            var hashedKey = SHA256.HashData(bytes);

            return Convert.ToBase64String(hashedKey);
        }
    }
}