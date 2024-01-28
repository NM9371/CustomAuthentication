using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using JWT.Serializers;
using System.Security.Cryptography;

namespace CustomAuthentication.Services.Implementations
{
    public class JWTHandler : ITokenHandler
    {
        private readonly IKey _key;

        public JWTHandler(IKey key)
        {
            _key = key;
        }

        public string GenerateToken()
        {
            RSA RSA256Key = _key.GetOrAdd();
            string token = JwtBuilder.Create()
                      .WithAlgorithm(new RS256Algorithm(RSA256Key, RSA256Key))
                      .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds())
                      .AddClaim("claim1", 0)
                      .AddClaim("claim2", "claim2-value")
                      .Encode();
            return token;
        }
        public string? VerifyToken(string token)
        {
            try
            {
                RSA RSA256Key = _key.GetOrAdd();
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new RS256Algorithm(RSA256Key, RSA256Key);
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                var json = decoder.Decode(token);
                Console.WriteLine(json);
            }
            catch (TokenNotYetValidException)
            {
                return "Token is not valid yet";
            }
            catch (TokenExpiredException)
            {
                return "Token has expired";
            }
            catch (SignatureVerificationException)
            {
                return "Token has invalid signature";
            }

            return null;
        }

    }
}