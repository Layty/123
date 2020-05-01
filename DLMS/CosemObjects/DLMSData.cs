using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class DLMSData : DLMSObject
    {
        public override string LogicalName { get; set; }
        public override byte Version { get; set; }
        public override ObjectType ObjectType { get; set; } = ObjectType.Data;

        public DLMSData(string logicalName)
        {
            LogicalName = logicalName;
        }

        public async Task<byte[]> GetValue()
        {
            return await GetAttributeData(2);
        }

        public void SetValue(DLMSDataItem ddi)
        {
            SetAttributeData(2, ddi);
        }
    }
}