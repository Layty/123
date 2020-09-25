namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class CosemAttributeDescriptorWithSelection :IToPduStringInHex, IPduStringInHexConstructor
    {
        public CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }
        public SelectiveAccessDescriptor SelectiveAccessDescriptor { get; set; }

        public CosemAttributeDescriptorWithSelection()
        {
        }

        public CosemAttributeDescriptorWithSelection(CosemAttributeDescriptor cosemAttributeDescriptor,
            SelectiveAccessDescriptor selectiveAccessDescriptor)
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            SelectiveAccessDescriptor = selectiveAccessDescriptor;
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            CosemAttributeDescriptor = new CosemAttributeDescriptor();
            if (!CosemAttributeDescriptor.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            if (pduStringInHex.StartsWith("00"))
            {
                pduStringInHex = pduStringInHex.Substring(2);
                SelectiveAccessDescriptor = null;
            }
            else
            {
                SelectiveAccessDescriptor = new SelectiveAccessDescriptor();
                if (!SelectiveAccessDescriptor.PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }
            return true;
        }

        public string ToPduStringInHex()
        {
            string str = CosemAttributeDescriptor.ToPduStringInHex();
            if (SelectiveAccessDescriptor != null)
            {
                return str + "01" + SelectiveAccessDescriptor.ToPduStringInHex();
            }
            return str + "00";
        }
    }
}