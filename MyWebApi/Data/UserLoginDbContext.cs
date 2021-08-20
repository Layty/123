using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities;

namespace MyWebApi.Data
{
    public class UserLoginDbContext : DbContext
    {
        public UserLoginDbContext(DbContextOptions<UserLoginDbContext> options) : base(options)
        {
        }

        public DbSet<UserLogin> UserLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLogin>().HasData(
                new UserLogin
                {
                    Id = 1,
                    UserName = "Admin",
                    Password = "123456"
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}