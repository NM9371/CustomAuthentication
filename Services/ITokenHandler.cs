namespace CustomAuthentication.Services
{
    public interface ITokenHandler
    {
        string GenerateToken();
        string? VerifyToken(string token);

    }
}