using CustomAuthentication.Services;
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

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
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
                return NotFound("Пользователь с таким логином не найден");
            }
           
            byte[] passwordSaltedHash = Hasher.GenerateHash(authorizeUserAs.HashSalt, user.Password);
            if (!authorizeUserAs.PasswordHash.SequenceEqual(passwordSaltedHash))
            {
                return BadRequest("Неверный пароль");
            }

            return Ok(authorizeUserAs);
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
                return BadRequest("Пользователь с таким логином уже зарегистрирован");
            }

            byte[] salt = Hasher.GenerateSalt();

            db.Users.Add(new User
                {
                    Login = user.Login,
                    Password = user.Password,
                    PasswordHash = Hasher.GenerateHash(salt, user.Password),
                    HashSalt = salt
            });
            await db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("ChangeSettings")]
        public async Task<ActionResult<User>> ChangeSettings(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            using var db = new ApplicationContext();

            if (!await db.Users.AsNoTracking().AnyAsync((x => x.Id == user.Id)))
            {
                return NotFound();
            }

            user.HashSalt = Hasher.GenerateSalt();
            user.PasswordHash = Hasher.GenerateHash(user.HashSalt, user.Password);

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