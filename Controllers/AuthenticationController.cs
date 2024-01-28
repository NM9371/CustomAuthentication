using CustomAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace CustomAuthentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {

        private readonly ILogger<AuthenticationController> _logger;
        private readonly IHasher _hasher;
        private readonly ITokenHandler _tokenWorker;
        private readonly IKey _key;

        public AuthenticationController(ILogger<AuthenticationController> logger, IHasher hasher, ITokenHandler tokenWorker, IKey key)
        {
            _hasher = hasher;
            _logger = logger;
            _tokenWorker = tokenWorker;
            _key = key;
        }

        [HttpPost("ValidateCredentials")]
        public async Task<ActionResult> ValidateCredentials(UserDTO user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            using var db = new ApplicationContext();
            User? authorizeUserAs = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login == user.Login);
            if (authorizeUserAs == null)
            {
                return NotFound("User with this login is not found");
            }

            if (!_hasher.VerifyHash(user.Password, authorizeUserAs.HashSalt, authorizeUserAs.PasswordHash))
            {
                return BadRequest("Wrong password");
            }

            var token = _tokenWorker.GenerateToken();

            return Ok(token);
        }

        [HttpPost("Registration")]
        public async Task<ActionResult> Registration(UserDTO user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            using var db = new ApplicationContext();

            if (await db.Users.AsNoTracking().AnyAsync(x => x.Login == user.Login))
            {
                return BadRequest("User with this login already exist");
            }

            db.Users.Add(new User
                {
                    Login = user.Login,
                    Password = user.Password,
                    PasswordHash = _hasher.GenerateHash(user.Password, out byte[] salt),
                    HashSalt = salt
                }
            );
            await db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("ChangeSettings")]
        public async Task<ActionResult<User>> ChangeSettings(UserPasswordUpdateModel userChanges)
        {
            if (userChanges == null)
            {
                return BadRequest();
            }

            string? tokenVerifyResult = _tokenWorker.VerifyToken(Request.Headers.Authorization);
            if (tokenVerifyResult != null) return BadRequest(tokenVerifyResult);

            using var db = new ApplicationContext();
            User? user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userChanges.Id);

            if (user == null)
            {
                return NotFound();
            }

            user.Login = userChanges.Login;
            user.Password = userChanges.Password;
            user.PasswordHash = _hasher.GenerateHash(user.Password, out byte[] salt);
            user.HashSalt = salt;

            db.Update(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("RemoveUser")]
        public async Task<ActionResult<User>> Delete(Guid userId)
        {
            using var db = new ApplicationContext();

            User? userToRemove = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (userToRemove == null)
            {
                return NotFound();
            }
            db.Users.Remove(userToRemove);
            await db.SaveChangesAsync();
            return Ok(userToRemove);
        }
    }
}