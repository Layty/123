using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class CosemProfileGeneric : CosemObject, IDLMSBase, INotifyPropertyChanged
    {
        public DLMSArray Buffer { get; set; } //2
        public object From { get; set; }
        public object To { get; set; }
        public List<KeyValuePair<CosemObject, DLMSCaptureObject>> CaptureObjects { get; set; } //3

        public uint CapturePeriod
        {
            get => _capturePeriod;
            set
            {
                _capturePeriod = value;
                OnPropertyChanged();
            }
        }

        private uint _capturePeriod;

        public AxdrUnsigned32 CapturePeriod1
        {
            get => _capturePeriod1;
            set
            {
                _capturePeriod1 = value;
                OnPropertyChanged();
            }
        } 

        private AxdrUnsigned32 _capturePeriod1=new AxdrUnsigned32();

        //5

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

        //6
        public CosemObject SortObject
        {
            get => _sortObject;
            set
            {
                _sortObject = value;
                OnPropertyChanged();
            }
        }

        private CosemObject _sortObject;


        public int SortAttributeIndex { get; set; }
        public int SortDataIndex { get; set; }

        public AccessRange AccessSelector { get; set; }

        /// <summary>
        /// 加载的条目数 //7
        /// </summary>

        public uint EntriesInUse
        {
            get => _entriesInUse;
            set
            {
                _entriesInUse = value;
                OnPropertyChanged();
            }
        }

        private uint _entriesInUse;


        /// <summary>
        /// 保持最大条目数   //8
        /// </summary>
        public uint ProfileEntries
        {
            get => _profileEntries;
            set
            {
                _profileEntries = value;
                OnPropertyChanged();
            }
        }

        private uint _profileEntries;

        public CosemProfileGeneric(string logicalName)
        {
            LogicalName = logicalName;
            ObjectType = ObjectType.ProfileGeneric;
            SortMethod = SortMethod.FiFo;
        }


        public AttributeDescriptor GetBufferAttributeDescriptor() => GetCosemAttributeDescriptor(2);

        public AttributeDescriptor GetCaptureObjectsAttributeDescriptor() => GetCosemAttributeDescriptor(3);

        public AttributeDescriptor GetCapturePeriodAttributeDescriptor() => GetCosemAttributeDescriptor(4);

        public byte[] SetCapturePeriod(uint capturePeriod)
        {
            this.CapturePeriod = capturePeriod;
            DLMSDataItem dlmsDataItem =
                new DLMSDataItem(this.GetDataType(4), BitConverter.GetBytes(CapturePeriod).Reverse().ToArray());
           this.CapturePeriod1=new AxdrUnsigned32();
           
            return SetAttributeData(4, dlmsDataItem);
        }

        public byte[] GetSortMethod() => GetAttributeData(5);
        public AttributeDescriptor GetSortMethodAttributeDescriptor() => GetCosemAttributeDescriptor(5);

        public byte[] GetEntriesInUse() => GetCosemAttributeDescriptor(7).ToPduBytes();
        public AttributeDescriptor GetEntriesInUseAttributeDescriptor() => GetCosemAttributeDescriptor(7);

        public byte[] GetProfileEntries() => GetAttributeData(8);
        public AttributeDescriptor GetProfileEntriesAttributeDescriptor() => GetCosemAttributeDescriptor(8);

        string[] IDLMSBase.GetNames()
        {
            return new string[8]
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

        int IDLMSBase.GetAttributeCount() => 8;

        int IDLMSBase.GetMethodCount() => 2;

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

        public void Reset()
        {
            DLMSDataItem dataItem = new DLMSDataItem(DataType.UInt8, new byte[] {00});
            ActionExecute(1, dataItem);
        }

        public void Capture()
        {
            DLMSDataItem dataItem = new DLMSDataItem(DataType.Int8, new byte[] {00});
            ActionExecute(2, dataItem);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}