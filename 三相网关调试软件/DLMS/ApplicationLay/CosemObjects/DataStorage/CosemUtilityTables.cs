using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemUtilityTables : CosemObject, IDlmsBase
    {
        public AxdrIntegerUnsigned16 TableId
        {
            get => _tableId;
            set
            {
                _tableId = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned16 _tableId;


        public AxdrIntegerUnsigned32 Length
        {
            get => _length;
            set
            {
                _length = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned32 _length;

        public AxdrIntegerOctetString Buffer
        {
            get => _buffer;
            set
            {
                _buffer = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerOctetString _buffer;


        public CosemUtilityTables()
        {
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.UtilityTables);
            TableId = new AxdrIntegerUnsigned16();
            Length = new AxdrIntegerUnsigned32();
            Buffer = new AxdrIntegerOctetString();
        }

        public CosemUtilityTables(string logicName) : this()
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