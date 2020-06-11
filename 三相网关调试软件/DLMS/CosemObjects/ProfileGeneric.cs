using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class ProfileGeneric : DLMSObject, IDLMSBase,INotifyPropertyChanged
    {
        public DLMSArray Buffer { get; set; } //2
        public object From { get; set; }
        public object To { get; set; }
        public List<KeyValuePair<DLMSObject, DLMSCaptureObject>> CaptureObjects { get; set; } //3


        public uint CapturePeriod
        {
            get => _CapturePeriod;
            set { _CapturePeriod = value; OnPropertyChanged(); }
        }
        private uint _CapturePeriod;


         //5

        [DefaultValue(SortMethod.FiFo)]
        public SortMethod SortMethod
        {
            get => _SortMethod;
            set { _SortMethod = value; OnPropertyChanged(); }
        }
        private SortMethod _SortMethod;

       //6
       public DLMSObject SortObject
        {
            get => _SortObject;
            set { _SortObject = value; OnPropertyChanged(); }
        }
        private DLMSObject _SortObject;


        public int SortAttributeIndex { get; set; }
        public int SortDataIndex { get; set; }

        public AccessRange AccessSelector { get; set; }

        /// <summary>
        /// 加载的条目数 //7
        /// </summary>

        public uint EntriesInUse
        {
            get => _EntriesInUse;
            set { _EntriesInUse = value; OnPropertyChanged(); }
        }
        private uint _EntriesInUse;


        /// <summary>
        /// 保持最大条目数   //8
        /// </summary>

        public uint ProfileEntries
        {
            get => _ProfileEntries;
            set { _ProfileEntries = value; OnPropertyChanged(); }
        }
        private uint _ProfileEntries;


        //public override byte Version { get; set; } = 1;

        public ProfileGeneric(string logicalName)
        {
            SortMethod = SortMethod.FiFo;
            LogicalName = logicalName;
            ObjectType = ObjectType.ProfileGeneric;
            //Version = 1;
        }


        public byte[] GetBuffer() => GetAttributeData(2);

        public byte[] GetCaptureObjects() => GetAttributeData(3);

        public byte[] GetCapturePeriod() => GetAttributeData(4);


        public byte[] SetCapturePeriod(uint capturePeriod)
        {
            this.CapturePeriod = capturePeriod;
            DLMSDataItem dlmsDataItem =
                new DLMSDataItem(this.GetDataType(4), BitConverter.GetBytes(CapturePeriod).Reverse().ToArray());
            return SetAttributeData(4, dlmsDataItem);
        }

        public byte[] GetSortMethod() => GetAttributeData(5);

        public byte[] GetEntriesInUse() => GetAttributeData(7);


        public byte[] GetProfileEntries() => GetAttributeData(8);


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