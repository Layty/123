using System.Collections.Generic;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Axdr;
using ClassLibraryDLMS.DLMS.Ber;
using ClassLibraryDLMS.DLMS.Common;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.CosemObjects
{
    public class CosemLoadIdentification : CosemObject
    {
        public CosemLoadIdentification()
        {
            LogicalName = "0.1.128.0.0.255";
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.LoadIdentification);
        }

        public CosemAttributeDescriptor GetCosemLoadIdentificationAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(2);
        }

        public SelectiveAccessDescriptor GetEarliestAccessDescriptor()
        {
            DLMSDataItem[] dataItems = new DLMSDataItem[]
            {
                new DLMSDataItem(DataType.UInt16, "0000"),
                new DLMSDataItem(DataType.UInt16, "0001"),
            };
            List<byte> list = new List<byte>();
            list.Add((byte) dataItems.Length);
            foreach (var dlmsDataItem in dataItems)
            {
                list.AddRange((dlmsDataItem.ToPduBytes()));
                ;
            }

            return new SelectiveAccessDescriptor(new AxdrUnsigned8("02"),
                new DLMSDataItem(DataType.Structure, list.ToArray()));
        }

        public SelectiveAccessDescriptor GetLatestAccessDescriptor()
        {
            DLMSDataItem[] dataItems = new DLMSDataItem[]
            {
                new DLMSDataItem(DataType.UInt16, "0001"),
                new DLMSDataItem(DataType.UInt16, "0001"),
            };
            List<byte> list = new List<byte>();
            list.Add((byte) dataItems.Length);
            foreach (var dlmsDataItem in dataItems)
            {
                list.AddRange(dlmsDataItem.ToPduBytes());
            }

            return new SelectiveAccessDescriptor(new AxdrUnsigned8("02"),
                new DLMSDataItem(DataType.Structure, list.ToArray()));
        }

        public SelectiveAccessDescriptor GetSelectiveAccessDescriptorWithTime(CosemClock cosemClock)
        {
            BerOctetString timerBerOctetString = new BerOctetString();
            timerBerOctetString.Value = cosemClock.GetDateTimeBytes().ByteToString("");

            DLMSDataItem[] dataItems = new[]
            {
                new DLMSDataItem(DataType.OctetString, timerBerOctetString.ToPduStringInHex()),
                new DLMSDataItem(DataType.UInt16, "0001"),
            };
            List<byte> list = new List<byte>();
            list.Add((byte) dataItems.Length);
            foreach (var dlmsDataItem in dataItems)
            {
                list.AddRange(dlmsDataItem.ToPduBytes());
            }

            return new SelectiveAccessDescriptor(new AxdrUnsigned8("01"),
                new DLMSDataItem(DataType.Structure, list.ToArray()));
        }

        public CosemAttributeDescriptorWithSelection GetLoadIdentificationWithTime(CosemClock cosemClock)
        {
            return new CosemAttributeDescriptorWithSelection(GetCosemLoadIdentificationAttributeDescriptor(),
                GetSelectiveAccessDescriptorWithTime(cosemClock));
        }

        public CosemAttributeDescriptorWithSelection GetLatestLoadIdentification()
        {
            return new CosemAttributeDescriptorWithSelection(GetCosemLoadIdentificationAttributeDescriptor(),
                GetLatestAccessDescriptor());
        }

        public CosemAttributeDescriptorWithSelection GetEarliestLoadIdentification()
        {
            return new CosemAttributeDescriptorWithSelection(GetCosemLoadIdentificationAttributeDescriptor(),
                GetEarliestAccessDescriptor());
        }
    }
}