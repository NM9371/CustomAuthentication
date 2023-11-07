using Microsoft.EntityFrameworkCore;

namespace CustomAuthentication
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=authentication;Username=postgres;Password=321");
        }
    }
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}