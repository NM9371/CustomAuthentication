using System.Security.Cryptography;

namespace CustomAuthentication.Services.Implementations
{
    public class RSA256Key : IKey
    {
        public RSA RsaKey { get; set; }
        public RSA GetOrAdd()
        {
           if (RsaKey == null)
            {
                RsaKey = RSA.Create();
            }

            return RsaKey;
        }
    }
}