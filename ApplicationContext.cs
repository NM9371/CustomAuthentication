using Microsoft.EntityFrameworkCore;
using Models;

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
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=authentication;Username=postgres;Password=321");
        }
    }
}