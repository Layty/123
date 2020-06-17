using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class DLMSUtilityTables : DLMSObject, IDLMSBase
    {
        public ushort TableId { get; set; }
        public byte[] Buffer { get; set; }

        public DLMSUtilityTables()
        {
            ObjectType = ObjectType.UtilityTables;
        }
        public byte[] GetLogicName() => GetAttributeData(1);

        public byte[] GetTableId() => GetAttributeData(2);

        public byte[] GetLength() => GetAttributeData(3);

        public byte[] GetBuffer() => GetAttributeData(4);

        string[] IDLMSBase.GetNames() => new[]
        {
            LogicalName,
            "Table Id",
            "Length",
            "Buffer"
        };


        int IDLMSBase.GetAttributeCount() => 4;

        int IDLMSBase.GetMethodCount() => 0;


        public DataType GetDataType(int index)
        {
            switch (index)
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
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }
    }
}