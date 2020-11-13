using System.Linq;
using System.Threading.Tasks;

namespace DlmsWebApi.Models
{
    public class DlmsHelper
    {
        public readonly CosemContext CosemContext;

        public DlmsHelper(CosemContext cosemContext)
        {
            this.CosemContext = cosemContext;
        }

        public async Task InitDataBaseAsync()
        {
            CosemContext.Database.EnsureCreated();
//            await InitDefaultDataAsync();
        }

       
    }
}