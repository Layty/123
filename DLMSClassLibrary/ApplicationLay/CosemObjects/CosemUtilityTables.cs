using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class CosemUtilityTables : CosemObject, IDLMSBase
    {
        public ushort TableId { get; set; }
        public byte[] Buffer { get; set; }

        public CosemUtilityTables()
        {
            ObjectType = ObjectType.UtilityTables;
        }

        public AttributeDescriptor GetLogicNameAttributeDescriptor() => GetCosemAttributeDescriptor(1);
        public AttributeDescriptor GetTableIdAttributeDescriptor() => GetCosemAttributeDescriptor(2);
        public AttributeDescriptor GetLengthAttributeDescriptor() => GetCosemAttributeDescriptor(3);
        public AttributeDescriptor GetBufferAttributeDescriptor() => GetCosemAttributeDescriptor(4);

        string[] IDLMSBase.GetNames() => new[]
        {
            LogicalName,
            "Table Id",
            "Length",
            "Buffer"
        };


        int IDLMSBase.GetAttributeCount() => 4;

        int IDLMSBase.GetMethodCount() => 0;

       public DataType GetDataType(int attrIndex)
        {
            switch (attrIndex)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.UInt16;
                case 3:
                    return DataType.UInt32;
                case 4:
                    return DataType.OctetString;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute Index.");
            }
        }
    }
}