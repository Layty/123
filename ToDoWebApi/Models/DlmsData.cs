using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DlmsWebApi.Models
{
    public interface IDlmsDataRepository
    {
        IEnumerable<DlmsData> GetAll();
        Task<DlmsData> Get(string obis);
        Task<DlmsData> Add(DlmsData item);
        void Remove(string obis);
        bool Update(DlmsData item);
    }

    public class DlmsData
    {
        [Key] public int Id { get; set; }

        public string DataName { get; set; }
        public int ClassId { get; set; }
        public string LogicName { get; set; }
    }
}