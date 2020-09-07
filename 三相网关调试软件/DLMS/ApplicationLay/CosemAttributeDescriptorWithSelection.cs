using System.Collections.Generic;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class CosemAttributeDescriptorWithSelection : IToPduBytes, IPduBytesToConstructor, IPduStringInHexConstructor
    {
        public AttributeDescriptor AttributeDescriptor { get; set; }
        public SelectiveAccessDescriptor SelectiveAccessDescriptor { get; set; }

        public CosemAttributeDescriptorWithSelection()
        {
        }

        public CosemAttributeDescriptorWithSelection(AttributeDescriptor attributeDescriptor,
            SelectiveAccessDescriptor selectiveAccessDescriptor)
        {
            AttributeDescriptor = attributeDescriptor;
            SelectiveAccessDescriptor = selectiveAccessDescriptor;
        }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            if (AttributeDescriptor != null)
            {
                list.AddRange(AttributeDescriptor.ToPduBytes());
                list.Add(0x01);
            }

            if (SelectiveAccessDescriptor != null)
            {
                list.AddRange(SelectiveAccessDescriptor.ToPduBytes());
            }

            return list.ToArray();
        }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            AttributeDescriptor = new AttributeDescriptor();
            if (!AttributeDescriptor.PduBytesToConstructor(pduBytes))
            {
                return false;
            }

            //if (pduBytes.Skip())
            //{

            //}
            return true;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            throw new System.NotImplementedException();
        }
    }
}