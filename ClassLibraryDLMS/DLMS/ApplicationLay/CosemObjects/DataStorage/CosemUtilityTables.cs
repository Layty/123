using System;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Common;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemUtilityTables : CosemObject, IDlmsBase
    {
        public ushort TableId { get; set; }
        public byte[] Buffer { get; set; }

        public CosemUtilityTables()
        {
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.UtilityTables);
        }

        public CosemUtilityTables(string logicName):this()
        {
            LogicalName = logicName;
        }
        public CosemAttributeDescriptor GetLogicNameAttributeDescriptor() => GetCosemAttributeDescriptor(1);
        public CosemAttributeDescriptor GetTableIdAttributeDescriptor() => GetCosemAttributeDescriptor(2);
        public CosemAttributeDescriptor GetLengthAttributeDescriptor() => GetCosemAttributeDescriptor(3);
        public CosemAttributeDescriptor GetBufferAttributeDescriptor() => GetCosemAttributeDescriptor(4);

        string[] IDlmsBase.GetNames() => new[]
        {
            LogicalName,
            "Table Id",
            "Length",
            "Buffer"
        };


        int IDlmsBase.GetAttributeCount() => 4;

        int IDlmsBase.GetMethodCount() => 0;

        DataType IDlmsBase.GetDataType(int attrIndex)
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