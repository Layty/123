using System;
using System.Collections.Generic;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    /// <summary>
    /// 通用表
    /// </summary>
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

        public AxdrOctetString Buffer
        {
            get => _buffer;
            set
            {
                _buffer = value;
                OnPropertyChanged();
            }
        }

        private AxdrOctetString _buffer;


        public CosemUtilityTables()
        {
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.UtilityTables);
            Version = 0;
            TableId = new AxdrIntegerUnsigned16();
            Length = new AxdrIntegerUnsigned32();
            Buffer = new AxdrOctetString();
        }

        public CosemUtilityTables(string logicName) : this()
        {
            LogicalName = logicName;
        }

        public CosemAttributeDescriptor LogicNameAttributeDescriptor => GetCosemAttributeDescriptor(1);
        public CosemAttributeDescriptor TableIdAttributeDescriptor => GetCosemAttributeDescriptor(2);
        public CosemAttributeDescriptor LengthAttributeDescriptor => GetCosemAttributeDescriptor(3);
        public CosemAttributeDescriptor BufferAttributeDescriptor => GetCosemAttributeDescriptor(4);

        public CosemAttributeDescriptorWithSelection GetBufferAttributeDescriptorWithSelectionByOffsetAccess()
        {
            return new CosemAttributeDescriptorWithSelection(BufferAttributeDescriptor,
                new SelectiveAccessDescriptor(new AxdrIntegerUnsigned8("01"),
                    CosemUtilityTablesBufferOffsetSelector.ToDlmsDataItem()));
        }

        public CosemAttributeDescriptorWithSelection GetBufferAttributeDescriptorWithSelectionByIndexAccess()
        {
            return new CosemAttributeDescriptorWithSelection(BufferAttributeDescriptor,
                new SelectiveAccessDescriptor(new AxdrIntegerUnsigned8("02"),
                    CosemUtilityTablesBufferIndexSelector.ToDlmsDataItem()));
        }

        public OffsetSelector CosemUtilityTablesBufferOffsetSelector { get; set; }
        public IndexSelector CosemUtilityTablesBufferIndexSelector { get; set; }

        public class OffsetSelector : IToDlmsDataItem
        {
            /// <summary>
            /// 访问区起始位置的偏移量(字节数),偏移量相对于表的开始
            /// </summary>
            public AxdrIntegerUnsigned32 Offset { get; set; }

            /// <summary>
            /// 请求或传输的字节数
            /// </summary>
            public AxdrIntegerUnsigned16 Count { get; set; }

            public DlmsDataItem ToDlmsDataItem()
            {
                throw new NotImplementedException();
            }
        }

        public class IndexSelector : IToDlmsDataItem
        {
            public List<AxdrIntegerUnsigned16> Index { get; set; }
            public AxdrIntegerUnsigned16 Count { get; set; }

            public DlmsDataItem ToDlmsDataItem()
            {
                throw new NotImplementedException();
            }
        }

        string[] IDlmsBase.GetNames() => new[]
        {
            LogicalName,
            "Table Id",
            "Length",
            "Buffer"
        };


        int IDlmsBase.AttributeCount => 4;

        int IDlmsBase.MethodCount => 0;

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