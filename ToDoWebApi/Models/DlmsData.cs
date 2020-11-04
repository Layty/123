using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Models
{
    public interface IIDlmsDataRepository
    {
        IEnumerable<DlmsData> GetAll();
        DlmsData Get(int id);
        DlmsData Add(DlmsData item);
        void Remove(int id);
        bool Update(DlmsData item);
    }

    public class DlmsData
    {
        [Key] public int Id { get; set; }

        public string DataName { get; set; }
        public string ClassId { get; set; }
        public string LogicName { get; set; }
        public byte Attr { get; set; }
    }
}