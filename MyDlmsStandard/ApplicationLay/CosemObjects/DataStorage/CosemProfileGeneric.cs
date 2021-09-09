using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemProfileGeneric : CosemObject, IDlmsBase
    {
        /// <summary>
        /// 曲线buffer,属性2
        /// </summary>
        public ObservableCollection<DlmsStructure> Buffer
        {
            get => _buffer;
            set
            {
                _buffer = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DlmsStructure> _buffer;

        /// <summary>
        /// 捕获对象，属性3
        /// </summary>
        public ObservableCollection<CaptureObjectDefinition> CaptureObjects { get; set; } =
            new ObservableCollection<CaptureObjectDefinition>();

        /// <summary>
        /// 捕获周期，单位为秒，属性4
        /// </summary>
        public AxdrIntegerUnsigned32 CapturePeriod
        {
            get => _capturePeriod;
            set
            {
                _capturePeriod = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned32 _capturePeriod = new AxdrIntegerUnsigned32();

  
        /// <summary>
        /// 排序方法，属性5，DefaultValue=SortMethod.FiFo
        /// </summary>
        [DefaultValue(SortMethod.FiFo)]
        public SortMethod SortMethod
        {
            get => _sortMethod;
            set
            {
                _sortMethod = value;
                OnPropertyChanged();
            }
        }

        private SortMethod _sortMethod;

        /// <summary>
        /// 排序对象，属性6，排序对象为捕获对象的一个子集，实际应该默认的话以时间排序对象；Clock:8-0.0.1.0.0.255-2
        /// </summary>
        public CaptureObjectDefinition SortObject
        {
            get => _sortObject;
            set
            {
                _sortObject = value;
                OnPropertyChanged();
            }
        }

        private CaptureObjectDefinition _sortObject;


        public AccessRange AccessSelector { get; set; }

        /// <summary>
        /// 已使用的条目数，属性7
        /// </summary>

        public AxdrIntegerUnsigned32 EntriesInUse
        {
            get => _entriesInUse;
            set
            {
                _entriesInUse = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned32 _entriesInUse = new AxdrIntegerUnsigned32();


        /// <summary>
        /// 保持最大条目数  ，属性8
        /// </summary>
        public AxdrIntegerUnsigned32 ProfileEntries
        {
            get => _profileEntries;
            set
            {
                _profileEntries = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned32 _profileEntries = new AxdrIntegerUnsigned32();

        public CosemProfileGeneric(string logicalName)
        {
            LogicalName = logicalName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.ProfileGeneric);
            SortMethod = SortMethod.FiFo;
            ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor();
            ProfileGenericEntryDescriptor = new ProfileGenericEntryDescriptor();
            Buffer = new ObservableCollection<DlmsStructure>();
        }


        public ProfileGenericRangeDescriptor ProfileGenericRangeDescriptor
        {
            get => _profileGenericRangeDescriptor;
            set { _profileGenericRangeDescriptor = value; OnPropertyChanged(); }
        }
        private ProfileGenericRangeDescriptor _profileGenericRangeDescriptor;


        public ProfileGenericEntryDescriptor ProfileGenericEntryDescriptor
        {
            get => _profileGenericEntryDescriptor;
            set { _profileGenericEntryDescriptor = value; OnPropertyChanged(); }
        }
        private ProfileGenericEntryDescriptor _profileGenericEntryDescriptor;

     

        public CosemAttributeDescriptor BufferAttributeDescriptor => GetCosemAttributeDescriptor(2);


        public CosemAttributeDescriptorWithSelection GetBufferAttributeDescriptorWithSelectionByRange()
        {
            return new CosemAttributeDescriptorWithSelection(BufferAttributeDescriptor,
                new SelectiveAccessDescriptor(new AxdrIntegerUnsigned8("01"),
                    ProfileGenericRangeDescriptor.ToDlmsDataItem()));
        }

        public CosemAttributeDescriptorWithSelection GetBufferAttributeDescriptorWithSelectionByEntry()
        {
            return new CosemAttributeDescriptorWithSelection(BufferAttributeDescriptor,
                new SelectiveAccessDescriptor(new AxdrIntegerUnsigned8("02"),
                    ProfileGenericEntryDescriptor.ToDlmsDataItem()));
        }

        public CosemAttributeDescriptor CaptureObjectsAttributeDescriptor => GetCosemAttributeDescriptor(3);

        public CosemAttributeDescriptor CapturePeriodAttributeDescriptor => GetCosemAttributeDescriptor(4);

        public CosemAttributeDescriptor SortMethodAttributeDescriptor => GetCosemAttributeDescriptor(5);

        public CosemAttributeDescriptor EntriesInUseAttributeDescriptor => GetCosemAttributeDescriptor(7);

        public CosemAttributeDescriptor ProfileEntriesAttributeDescriptor => GetCosemAttributeDescriptor(8);


        public CosemMethodDescriptor ResetMethodDescriptor => GetCosemMethodDescriptor(1);
        public CosemMethodDescriptor CaptureMethodDescriptor => GetCosemMethodDescriptor(2);

        string[] IDlmsBase.GetNames()
        {
            return new[]
            {
                LogicalName,
                "Buffer",
                "CaptureObjects",
                "Capture Period",
                "Sort Method",
                "Sort Object",
                "Entries In Use",
                "Profile Entries"
            };
        }

        int IDlmsBase.AttributeCount => 8;

        int IDlmsBase.MethodCount => 2;

        public DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Array;
                case 3:
                    return DataType.Array;
                case 4:
                    return DataType.UInt32;
                case 5:
                    return DataType.Enum;
                case 6:
                    return DataType.Array;
                case 7:
                    return DataType.UInt32;
                case 8:
                    return DataType.UInt32;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }


        #region 方法

        public virtual void Reset()
        {
            EntriesInUse.Value = "00000000";
            Buffer = new ObservableCollection<DlmsStructure>();
            // DlmsDataItem dataItem = new DlmsDataItem(DataType.UInt8) {Value = "00"};
//            ActionExecute(1, dataItem);
        }

        public void Capture(DlmsStructure dlmsStructure)
        {
            // EntriesInUse.Value + 1;
            Buffer.Add(dlmsStructure);
            // DlmsDataItem dataItem = new DlmsDataItem(DataType.Int8) {Value = "00"};
            //            ActionExecute(2, dataItem);
        }

        #endregion


        public int[] GetAttributeIndexToRead(bool all)
        {
            throw new NotImplementedException();
        }
    }
}