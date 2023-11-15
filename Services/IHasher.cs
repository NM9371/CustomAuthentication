namespace CustomAuthentication.Services
{
    public interface IHasher
    {
        byte[] GenerateHash(string plainTextPassword, out byte[] salt);
        bool VerifyHash(string plainTextPassword, byte[] salt, byte[] hash);

    }
}