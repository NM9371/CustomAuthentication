using System.Security.Cryptography;
using System.Text;

namespace CustomAuthentication.Services.Implementations
{
    public class SHA256Hasher : IHasher
    {
        private byte[] GenerateSalt(int SaltLength = 32)
        {
            byte[] salt = new byte[SaltLength];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }
            return salt;
        }
        public byte[] GenerateHash(string plainTextPassword, out byte[] salt)
        {
            salt = GenerateSalt();
            byte[] bytePassword = Encoding.UTF8.GetBytes(plainTextPassword);
            byte[] saltedPassword = bytePassword.Concat(salt).ToArray();
            byte[] hash = SHA256.HashData(saltedPassword);
            return hash;
        }
        public bool VerifyHash(string plainTextPassword, byte[] salt, byte[] hash)
        {
            byte[] bytePassword = Encoding.UTF8.GetBytes(plainTextPassword);
            byte[] saltedPassword = bytePassword.Concat(salt).ToArray();
            byte[] newHash = SHA256.HashData(saltedPassword);
            return hash.SequenceEqual(newHash);
        }

    }
}