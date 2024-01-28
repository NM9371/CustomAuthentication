using System.Security.Cryptography;

namespace CustomAuthentication.Services
{
    public interface IKey
    {
        RSA GetOrAdd();

    }
}