using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public interface IToDlmsDataItem
    {
        DLMSDataItem ToDlmsDataItem();
    }

    public class ProfileGenericRangeDescriptor : IToDlmsDataItem
    {
        public CaptureObjectDefinition RestrictingObject { get; set; }
        public DLMSDataItem FromValue { get; set; }
        public DLMSDataItem ToValue { get; set; }
        public List<CaptureObjectDefinition> SelectedValues { get; set; }

        public DLMSDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DLMSDataItem[4];
            dlmsStructure.Items[0] = RestrictingObject.ToDlmsDataItem();
            dlmsStructure.Items[1] = FromValue;
            dlmsStructure.Items[2] = ToValue;
            DLMSArray dlmsArray = new DLMSArray();
            if (SelectedValues == null || SelectedValues.Count == 0)
            {
                dlmsArray.Items = new DLMSDataItem[0];
            }
            else
            {
                dlmsArray.Items = new DLMSDataItem[SelectedValues.Count];
                for (int i = 0; i < SelectedValues.Count; i++)
                {
                    dlmsArray.Items[i] = SelectedValues[i].ToDlmsDataItem();
                }
            }

            dlmsStructure.Items[3] = new DLMSDataItem(DataType.Array, dlmsArray.ToPduStringInHex());

            DLMSDataItem dlmsDataItem = new DLMSDataItem(DataType.Structure, dlmsStructure.ToPduBytes());
            return dlmsDataItem;
        }
    }

    public class ProfileGenericEntryDescriptor : IToDlmsDataItem
    {
        public uint FromIndex { get; set; }
        public uint ToIndex { get; set; }
        public ushort FromSelectedValue { get; set; }
        public ushort ToSelectedValue { get; set; }

        public DLMSDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DLMSDataItem[4];
            dlmsStructure.Items[0] = new DLMSDataItem(DataType.UInt32, FromIndex.ToString("X8"));
            dlmsStructure.Items[1] = new DLMSDataItem(DataType.UInt32, ToIndex.ToString("X8"));
            dlmsStructure.Items[2] = new DLMSDataItem(DataType.UInt16, FromSelectedValue.ToString("X4"));
            dlmsStructure.Items[3] = new DLMSDataItem(DataType.UInt16, ToSelectedValue.ToString("X4"));
            DLMSDataItem dlmsDataItem = new DLMSDataItem(DataType.Structure, dlmsStructure.ToPduBytes());

            return dlmsDataItem;
        }
    }

    public class CosemProfileGeneric : CosemObject, IDlmsBase
    {
        public DLMSArray Buffer { get; set; } //2


        public ObservableCollection<CaptureObjectDefinition> CaptureObjects { get; set; } //3

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

        private AxdrUnsigned32 _capturePeriod1 = new AxdrUnsigned32();

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
            ClassId = MyConvert.GetClassIdByObjectType(objectType: ObjectType.ProfileGeneric);

            SortMethod = SortMethod.FiFo;
        }

        public ProfileGenericEntryDescriptor ProfileGenericEntryDescriptor { get; set; }

        public ProfileGenericRangeDescriptor ProfileGenericRangeDescriptor { get; set; }
        public CosemAttributeDescriptor GetBufferAttributeDescriptor() => GetCosemAttributeDescriptor(2);

        public CosemAttributeDescriptorWithSelection GetBufferAttributeDescriptorWithSelectionByEntry()
        {
            return new CosemAttributeDescriptorWithSelection(GetBufferAttributeDescriptor(),
                new SelectiveAccessDescriptor(new AxdrUnsigned8("01"), ProfileGenericEntryDescriptor.ToDlmsDataItem()));
        }

        public CosemAttributeDescriptorWithSelection GetBufferAttributeDescriptorWithSelectionByRange()
        {
            return new CosemAttributeDescriptorWithSelection(GetBufferAttributeDescriptor(),
                new SelectiveAccessDescriptor(new AxdrUnsigned8("01"), ProfileGenericRangeDescriptor.ToDlmsDataItem()));
        }

        public CosemAttributeDescriptor GetCaptureObjectsAttributeDescriptor() => GetCosemAttributeDescriptor(3);

        public CosemAttributeDescriptor GetCapturePeriodAttributeDescriptor() => GetCosemAttributeDescriptor(4);


//        public byte[] SetCapturePeriod(uint capturePeriod)
//        {
//            this.CapturePeriod = capturePeriod;
//            DLMSDataItem dlmsDataItem =
//                new DLMSDataItem(this.GetDataType(4), BitConverter.GetBytes(CapturePeriod).Reverse().ToArray());
//            this.CapturePeriod1 = new AxdrUnsigned32();
//
//            return SetAttributeData(4, dlmsDataItem);
//        }


        public CosemAttributeDescriptor GetSortMethodAttributeDescriptor() => GetCosemAttributeDescriptor(5);


        public CosemAttributeDescriptor GetEntriesInUseAttributeDescriptor() => GetCosemAttributeDescriptor(7);

        public CosemAttributeDescriptor GetProfileEntriesAttributeDescriptor() => GetCosemAttributeDescriptor(8);

        string[] IDlmsBase.GetNames()
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

        int IDlmsBase.GetAttributeCount() => 8;

        int IDlmsBase.GetMethodCount() => 2;

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
            DLMSDataItem dataItem = new DLMSDataItem(DataType.UInt8, new byte[] {00});
//            ActionExecute(1, dataItem);
        }

        public void Capture()
        {
            DLMSDataItem dataItem = new DLMSDataItem(DataType.Int8, new byte[] {00});
//            ActionExecute(2, dataItem);
        }

        #endregion


        public int[] GetAttributeIndexToRead(bool all)
        {
            throw new NotImplementedException();
        }
    }
}