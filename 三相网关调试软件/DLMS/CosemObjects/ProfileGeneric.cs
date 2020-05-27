using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class ProfileGeneric : DLMSObject, IDLMSBase
    {
      //  public override ObjectType ObjectType { get; set; } = ObjectType.ProfileGeneric;

        //public sealed override string LogicalName { get; set; } //1
        public List<object[]> Buffer { get; set; } //2
        public object From { get; set; }
        public object To { get; set; }
        public List<KeyValuePair<DLMSObject, DLMSCaptureObject>> CaptureObjects { get; set; } //3

        public uint CapturePeriod { get; set; } //4
        [DefaultValue(SortMethod.FiFo)] public SortMethod SortMethod { get; set; } //5
        public DLMSObject SortObject { get; set; } //6
        public int SortAttributeIndex { get; set; }
        public int SortDataIndex { get; set; }

        public AccessRange AccessSelector { get; set; }

        /// <summary>
        /// 加载的条目数
        /// </summary>
        public uint EntriesInUse { get; set; } //7

        /// <summary>
        /// 保持最大条目数
        /// </summary>
        public uint ProfileEntries { get; set; } //8


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
                new DLMSDataItem(GetDataType(4), BitConverter.GetBytes(CapturePeriod).Reverse().ToArray());
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
    }
}