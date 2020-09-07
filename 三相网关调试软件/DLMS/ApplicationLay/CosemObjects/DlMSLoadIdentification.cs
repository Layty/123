using System.Collections.Generic;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Ber;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class DlMSLoadIdentification : DLMSObject
    {
        public DlMSLoadIdentification()
        {
            LogicalName = "0.1.128.0.0.255";
            ObjectType = (ObjectType) 8301;
        }

        public CosemAttributeDescriptorWithSelection GetLoadIdentificationWithTime(DLMSClock dlmsClock)
        {
            Ber.BerOctetString timerBerOctetString=new BerOctetString();
            timerBerOctetString.Value = dlmsClock.GetDateTimeBytes().ByteToString("");
           
            DLMSDataItem[] dataItems = new[]
            {
                new DLMSDataItem(DataType.OctetString, timerBerOctetString.ToPduBytes()),
                new DLMSDataItem(DataType.UInt16,"0001"),
            };
            List<byte> list = new List<byte>();
            list.Add((byte)dataItems.Length);
            foreach (var dlmsDataItem in dataItems)
            {
                list.AddRange(dlmsDataItem.ToPduBytes()); ;
            }
            return new CosemAttributeDescriptorWithSelection(new AttributeDescriptor(this, 2),
                new SelectiveAccessDescriptor(1, new DLMSDataItem(DataType.Structure, list.ToArray())));
        }

        public CosemAttributeDescriptorWithSelection GetLatestLoadIdentification()
        {
            DLMSDataItem[] dataItems=new DLMSDataItem[]
            {
                new DLMSDataItem(DataType.UInt16,"0001"),
                new DLMSDataItem(DataType.UInt16,"0001"),
            };
            List<byte> list = new List<byte>();
            list.Add((byte)dataItems.Length);
            foreach (var dlmsDataItem in dataItems)
            {
                list.AddRange(dlmsDataItem.ToPduBytes()); ;
            }

            return new CosemAttributeDescriptorWithSelection(new AttributeDescriptor(this, 2),
                new SelectiveAccessDescriptor(2, new DLMSDataItem(DataType.Structure, list.ToArray())));
        }

        public CosemAttributeDescriptorWithSelection GetEarliestLoadIdentification()
        {
            DLMSDataItem[] dataItems = new DLMSDataItem[]
            {
                new DLMSDataItem(DataType.UInt16,"0000"),
                new DLMSDataItem(DataType.UInt16,"0001"),
            };
            List<byte> list = new List<byte>();
            list.Add((byte)dataItems.Length);
            foreach (var dlmsDataItem in dataItems)
            {
                list.AddRange(dlmsDataItem.ToPduBytes()); ;
            }

            return new CosemAttributeDescriptorWithSelection(new AttributeDescriptor(this, 2),
                new SelectiveAccessDescriptor(2, new DLMSDataItem(DataType.Structure, list.ToArray())));
        }
    }
}