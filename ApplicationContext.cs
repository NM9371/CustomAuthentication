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
            optionsBuilder.UseNpgsql("Host=authDB;Port=5432;Database=postgres;Username=postgres;Password=Master1234");
        }
    }
}