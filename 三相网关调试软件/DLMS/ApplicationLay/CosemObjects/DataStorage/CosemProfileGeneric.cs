﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemProfileGeneric : CosemObject, IDlmsBase
    {
        public DLMSArray Buffer { get; set; } //2

        public ObservableCollection<CaptureObjectDefinition> CaptureObjects { get; set; } =
            new ObservableCollection<CaptureObjectDefinition>(); //3

        public AxdrUnsigned32 CapturePeriod
        {
            get => _capturePeriod;
            set
            {
                _capturePeriod = value;
                OnPropertyChanged();
            }
        }

        private AxdrUnsigned32 _capturePeriod = new AxdrUnsigned32();

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

        public AxdrUnsigned32 EntriesInUse
        {
            get => _entriesInUse;
            set
            {
                _entriesInUse = value;
                OnPropertyChanged();
            }
        }

        private AxdrUnsigned32 _entriesInUse = new AxdrUnsigned32();


        /// <summary>
        /// 保持最大条目数   //Attribute=8
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

        public CosemAttributeDescriptor GetSortMethodAttributeDescriptor() => GetCosemAttributeDescriptor(5);

        public CosemAttributeDescriptor GetEntriesInUseAttributeDescriptor() => GetCosemAttributeDescriptor(7);

        public CosemAttributeDescriptor GetProfileEntriesAttributeDescriptor() => GetCosemAttributeDescriptor(8);


        public CosemMethodDescriptor GetResetMethodDescriptor() => GetCosemMethodDescriptor(1);
        public CosemMethodDescriptor GetCaptureMethodDescriptor() => GetCosemMethodDescriptor(2);
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
            DlmsDataItem dataItem = new DlmsDataItem(DataType.UInt8) {Value = "00"};
//            ActionExecute(1, dataItem);
        }

        public void Capture()
        {
            DlmsDataItem dataItem = new DlmsDataItem(DataType.Int8) {Value = "00"};
//            ActionExecute(2, dataItem);
        }

        #endregion


        public int[] GetAttributeIndexToRead(bool all)
        {
            throw new NotImplementedException();
        }
    }
}