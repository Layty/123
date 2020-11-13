using Microsoft.EntityFrameworkCore;

namespace DlmsWebApi.Models
{
    public class CosemContext : DbContext
    {
       
        public CosemContext(DbContextOptions<CosemContext> options):base(options)
        {
            
        }

        public DbSet<DlmsData> CosemItems { get; set; }
    }
}