using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;

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

        [HttpPost(Name = "ValidateCredentials")]
        public async Task<ActionResult> ValidateCredentials(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            using var db = new ApplicationContext();
            db.Users.FirstOrDefault(x => x.Login == user.Login);
            return Ok(user);
        }

        [HttpPost(Name = "Registration")]
        public async Task<ActionResult> Registration(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            using var db = new ApplicationContext();
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut(Name = "ChangeSettings")]
        public async Task<ActionResult<User>> ChangeSettings(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            using var db = new ApplicationContext();

            if (!db.Users.Any(x => x.Login == user.Login))
            {
                return NotFound();
            }
            db.Update(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }
    }
}