using System;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class DLMSUtilityTables : DLMSObject, IDLMSBase
    {
        public ushort TableId { get; set; }
        public byte[] Buffer { get; set; }

        public override ObjectType ObjectType { get; set; } = ObjectType.UtilityTables;

        public async Task<byte[]> GetLogicName()
        {
            return await GetAttributeData(1);
        }

        public async Task<byte[]> GetTableId()
        {
            return await GetAttributeData(2);
        }

        public async Task<byte[]> GetLength()
        {
            return await GetAttributeData(3);
        }

        public async Task<byte[]> GetBuffer()
        {
            return await GetAttributeData(4);
        }

        string[] IDLMSBase.GetNames() => new string[4]
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