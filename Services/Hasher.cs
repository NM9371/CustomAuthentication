using System.Security.Cryptography;
using System.Text;

namespace CustomAuthentication.Services
{
    public class Hasher
    {
        public static int SaltLength = 32;

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltLength];
            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            return salt;
        }
        public static byte[] GenerateHash(byte[] salt, string plainTextPassword)
        {
            byte[] bytePassword = Encoding.UTF8.GetBytes(plainTextPassword);
            byte[] saltedPassword = bytePassword.Concat(salt).ToArray();
            byte[] hash = SHA256.HashData(saltedPassword);
            return hash;
        }
    }
}