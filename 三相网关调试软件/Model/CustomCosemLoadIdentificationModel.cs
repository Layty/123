using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Ber;
using MyDlmsStandard.Common;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemLoadIdentificationModel : CosemObject
    {
        public CustomCosemLoadIdentificationModel()
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
            DlmsStructure structure = new DlmsStructure
            {
                Items = new[]
                {
                    new DlmsDataItem(DataType.UInt16, "0000"),
                    new DlmsDataItem(DataType.UInt16, "0001"),
                }
            };
            return new SelectiveAccessDescriptor(new AxdrIntegerUnsigned8("02"),
                new DlmsDataItem(DataType.Structure, structure));
        }

        public SelectiveAccessDescriptor GetLatestAccessDescriptor()
        {
            DlmsStructure structure = new DlmsStructure
            {
                Items = new[]
                {
                    new DlmsDataItem(DataType.UInt16) {Value = "0001"},
                    new DlmsDataItem(DataType.UInt16) {Value = "0001"},
                }
            };
            return new SelectiveAccessDescriptor(new AxdrIntegerUnsigned8("02"),
                new DlmsDataItem(DataType.Structure, structure));
        }

        public SelectiveAccessDescriptor GetSelectiveAccessDescriptorWithTime(CosemClock cosemClock)
        {
            BerOctetString timerBerOctetString = new BerOctetString
            {
                Value = cosemClock.GetDateTimeBytes().ByteToString("")
            };
            DlmsStructure dlmsStructure = new DlmsStructure
            {
                Items = new[]
                {
                  //  new DlmsDataItem(DataType.OctetString, timerBerOctetString.ToPduStringInHex()),这个是错误的
                     new DlmsDataItem(DataType.OctetString, timerBerOctetString.Value),
                    new DlmsDataItem(DataType.UInt16, "0001"),
                }
            };
            return new SelectiveAccessDescriptor(new AxdrIntegerUnsigned8("01"),
                new DlmsDataItem(DataType.Structure, dlmsStructure));
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