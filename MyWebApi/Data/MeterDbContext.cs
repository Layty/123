using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities;

namespace MyWebApi.Data
{
    public class MeterDbContext : DbContext
    {
        public MeterDbContext(DbContextOptions<MeterDbContext> options) : base(options)
        {
        }

        public DbSet<Meter> Meters { get; set; }
        public DbSet<Energy> Energies { get; set; }
        public DbSet<Power> Powers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Meter>().HasData(
                new Meter
                {
                    MeterId = "000000000001",
                });


            base.OnModelCreating(modelBuilder);
        }
    }
}