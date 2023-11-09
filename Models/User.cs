using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

}