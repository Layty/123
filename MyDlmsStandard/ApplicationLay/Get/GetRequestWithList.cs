using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequestWithList : IGetRequest
    {
        [XmlIgnore] public GetRequestType GetRequestType { get; } = GetRequestType.WithList;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        public CosemAttributeDescriptorWithSelection[] AttributeDescriptorList { get; set; }

        public GetRequestWithList()
        {
        }

        public GetRequestWithList(CosemAttributeDescriptorWithSelection[] attributeDescriptorList)
        {
            AttributeDescriptorList = attributeDescriptorList;
            InvokeIdAndPriority = new AxdrIntegerUnsigned8("C1");
        }

        //        public byte[] ToPduBytes()
        //        {
        //            List<byte> pduBytes = new List<byte>();
        //            pduBytes.Add((byte) GetRequestType);
        //            pduBytes.Add(InvokeIdAndPriority.GetEntityValue());
        //            int num = AttributeDescriptorList.Length;
        //            if (num < 127)
        //            {
        //                pduBytes.Add((byte) num);
        //            }
        //            else if (num < 255)
        //            {
        //                pduBytes.Add(0x81);
        //                pduBytes.Add((byte) num);
        //            }
        //            else
        //            {
        //                pduBytes.Add(0x82);
        //
        //                pduBytes.AddRange(BitConverter.GetBytes((byte) num).Reverse().ToArray());
        //            }
        //
        //            if (AttributeDescriptorList != null)
        //            {
        //                foreach (var cosemAttributeDescriptor in AttributeDescriptorList)
        //                {
        //                    pduBytes.AddRange(MyConvert.OctetStringToByteArray(cosemAttributeDescriptor.ToPduStringInHex()));
        //                }
        //            }
        //
        //            return pduBytes.ToArray();
        //        }

        public string GetRequestToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("03");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            int num = AttributeDescriptorList.Length;
            if (num <= 127)
            {
                stringBuilder.Append(num.ToString("X2"));
            }
            else if (num <= 255)
            {
                stringBuilder.Append("81" + num.ToString("X2"));
            }
            else
            {
                stringBuilder.Append("82" + num.ToString("X4"));
            }
            CosemAttributeDescriptorWithSelection[] array = AttributeDescriptorList;
            foreach (CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection in array)
            {
                stringBuilder.Append(cosemAttributeDescriptorWithSelection.ToPduStringInHex());
            }
            return stringBuilder.ToString();
        }
    }
}