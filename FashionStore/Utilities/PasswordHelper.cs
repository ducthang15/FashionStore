using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace FashionStore.Utilities
{
    public static class PasswordHelper
    {
        private static PasswordHasher<string> hasher = new PasswordHasher<string>();

        public static string Hash(string password)
        {
            return hasher.HashPassword(null, password);
        }

        public static bool Verify(string hashedPassword, string inputPassword)
        {
            var result = hasher.VerifyHashedPassword(null, hashedPassword, inputPassword);
            return result == PasswordVerificationResult.Success;
        }

        public static string GetMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = md5.ComputeHash(bytes);
                return Convert.ToHexString(hash).ToLower();
            }
        }
    }
}