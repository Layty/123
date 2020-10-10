namespace ClassLibraryDLMS.DLMS.ApplicationLay
{
    public class CosemAttributeDescriptorWithSelection :IToPduStringInHex, IPduStringInHexConstructor
    {
        public CosemAttributeDescriptor AttributeDescriptor { get; set; }
        public SelectiveAccessDescriptor SelectiveAccessDescriptor { get; set; }

        public CosemAttributeDescriptorWithSelection()
        {
        }

        public CosemAttributeDescriptorWithSelection(CosemAttributeDescriptor attributeDescriptor,
            SelectiveAccessDescriptor selectiveAccessDescriptor)
        {
            AttributeDescriptor = attributeDescriptor;
            SelectiveAccessDescriptor = selectiveAccessDescriptor;
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            AttributeDescriptor = new CosemAttributeDescriptor();
            if (!AttributeDescriptor.PduStringInHexConstructor(ref pduStringInHex))
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
            string str = AttributeDescriptor.ToPduStringInHex();
            if (SelectiveAccessDescriptor != null)
            {
                return str + "01" + SelectiveAccessDescriptor.ToPduStringInHex();
            }
            return str + "00";
        }
    }
}