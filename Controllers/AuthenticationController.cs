using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
            User? authorizeUserAs = db.Users.FirstOrDefault(x => x.Login == user.Login);
            if (authorizeUserAs == null)
            {
                return NotFound("������������ � ����� ������� �� ������");
            }

            byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password));
            if (!authorizeUserAs.PasswordHash.SequenceEqual(passwordHash))
            {
                return BadRequest("�������� ������");
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

            if (db.Users.Any(x => x.Login == user.Login))
            {
                return BadRequest("������������ � ����� ������� ��� ���������������");
            }

            db.Users.Add(new User
                {
                    Login = user.Login,
                    Password = user.Password,
                    PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password))
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

            if (!db.Users.Any(x => x.Id == user.Id))
            {
                return NotFound();
            }

            user.PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password));

            db.Update(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("RemoveUser")]
        public async Task<ActionResult<User>> Delete(Guid userId)
        {
            using var db = new ApplicationContext();

            User? removingUser = db.Users.FirstOrDefault(x => x.Id == userId);
            if (removingUser == null)
            {
                return NotFound();
            }
            db.Users.Remove(removingUser);
            await db.SaveChangesAsync();
            return Ok(removingUser);
        }
    }
}