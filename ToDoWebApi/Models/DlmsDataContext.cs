using Microsoft.EntityFrameworkCore;

namespace DlmsWebApi.Models
{
    public class DlmsDataContext : DbContext
    {
       
        public DlmsDataContext(DbContextOptions<DlmsDataContext> options):base(options)
        {
            
        }

        public DbSet<DlmsData> DlmsDataItems { get; set; }
    }
}