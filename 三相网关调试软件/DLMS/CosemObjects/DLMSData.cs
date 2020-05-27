using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class DLMSData : DLMSObject
    {
        //public override string LogicalName { get; set; }
        //public override byte Version { get; set; }
        //public override ObjectType ObjectType { get; set; } = ObjectType.Data;

        public DLMSData(string logicalName)
        {
           this.LogicalName = logicalName;
           ObjectType = ObjectType.Data;
        }

        public byte[] GetValue()
        {
            return GetAttributeData(2);
        }

        public byte[] SetValue(DLMSDataItem ddi)
        {
            return SetAttributeData(2, ddi);
        }
    }
}