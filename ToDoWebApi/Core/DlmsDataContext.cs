using Microsoft.EntityFrameworkCore;

namespace ToDoWebApi.Core
{
    public class DlmsDataContext : DbContext
    {
       
        public DlmsDataContext(DbContextOptions<DlmsDataContext> options):base(options)
        {
            
        }

        public DbSet<DlmsData> DlmsDatas { get; set; }
    }
}