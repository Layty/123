using System.Collections.Generic;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetRequestNormal : IToPduBytes
    {
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.Normal;
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
       =new AxdrUnsigned8("C1");
        public CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }
        public SelectiveAccessDescriptor AccessSelection { get; set; }
        public CosemAttributeDescriptorWithSelection CosemAttributeDescriptorWithSelection { get; set; }

        public GetRequestNormal()
        {
        }

        public GetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor)
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
        }


        public GetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor,
            SelectiveAccessDescriptor accessSelection)
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            AccessSelection = accessSelection;
         
        }

        public GetRequestNormal(CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection)
        {
            CosemAttributeDescriptorWithSelection = cosemAttributeDescriptorWithSelection;
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.GetEntityValue());
            if (CosemAttributeDescriptor != null)
            {
                pduBytes.AddRange(CosemAttributeDescriptor.ToPduStringInHex().StringToByte());
                pduBytes.Add(0x00);
            }

            if (AccessSelection != null)
            {
                pduBytes.Add(0x01);
                pduBytes.AddRange(AccessSelection.ToPduStringInHex().StringToByte());
            }

            if (CosemAttributeDescriptorWithSelection != null)
            {
                pduBytes.AddRange(CosemAttributeDescriptorWithSelection.ToPduStringInHex().StringToByte());
            }

            return pduBytes.ToArray();
        }
    }
}