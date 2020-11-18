using System;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities;

namespace MyWebApi.Data
{
    public class CosemObjectDbContext : DbContext
    {
        public CosemObjectDbContext(DbContextOptions<CosemObjectDbContext> options) : base(options)
        {
        }

        public DbSet<CosemObject> CosemObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CosemObject>().HasData(
                new CosemObject
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    ClassId = 1,
                    Name = "Local Time",
                    Obis = "1.0.0.9.1.255"
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}