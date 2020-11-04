using System.Linq;
using System.Threading.Tasks;

namespace DlmsWebApi.Models
{
    public class DlmsHelper
    {
        public readonly DlmsDataContext DlmsDataContext;

        public DlmsHelper(DlmsDataContext dlmsDataContext)
        {
            this.DlmsDataContext = dlmsDataContext;
        }

        public async Task InitDataBaseAsync()
        {
            DlmsDataContext.Database.EnsureCreated();
//            await InitDefaultDataAsync();
        }

        async Task InitDefaultDataAsync()
        {
            if (!this.DlmsDataContext.DlmsDataItems.Any())
            {
                DlmsDataContext.DlmsDataItems.Add(new DlmsData()
                    {Id = 1, Attr = 1, ClassId = "0001", DataName = "ShuJu", LogicName = "123"});
            }

            await DlmsDataContext.SaveChangesAsync();
        }
    }
}