using System.Linq;
using System.Threading.Tasks;

namespace ToDoWebApi.Core
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
            if (!this.DlmsDataContext.DlmsDatas.Any())
            {
                DlmsDataContext.DlmsDatas.Add(new DlmsData()
                    {Id = 1, Attr = 1, ClassId = "0001", DataName = "ShuJu", LogicName = "123"});
            }

            await DlmsDataContext.SaveChangesAsync();
        }
    }
}