using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequestNormal : IGetRequest
    {
        [XmlIgnore] public GetRequestType GetRequestType { get; } = GetRequestType.Normal;

        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; } = new AxdrIntegerUnsigned8("C1");

        public CosemAttributeDescriptor AttributeDescriptor { get; set; }
        public SelectiveAccessDescriptor AccessSelection { get; set; }
        public CosemAttributeDescriptorWithSelection AttributeDescriptorWithSelection { get; set; }

        public GetRequestNormal()
        {

        }
        public GetRequestNormal(CosemAttributeDescriptor attributeDescriptor)
        {
            AttributeDescriptor = attributeDescriptor;
        }



        public GetRequestNormal(CosemAttributeDescriptorWithSelection attributeDescriptorWithSelection)
        {
            AttributeDescriptorWithSelection = attributeDescriptorWithSelection;
        }

        public string GetRequestToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("01");
            if (InvokeIdAndPriority != null)
            {
                stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            }

            if (AttributeDescriptor != null)
            {
                stringBuilder.Append(AttributeDescriptor.ToPduStringInHex());
                stringBuilder.Append("00");
            }

            if (AccessSelection != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(AccessSelection.ToPduStringInHex());
            }

            if (AttributeDescriptorWithSelection != null)
            {
                stringBuilder.Append(AttributeDescriptorWithSelection.ToPduStringInHex());
            }

            return stringBuilder.ToString();
        }

        internal bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
            string getRequestType = pduStringInHex.Substring(0, 2);
            if (getRequestType != "01")
            {
                return false;
            }
            string InvokeIdAndPriority = pduStringInHex.Substring(2, 2);
            if (InvokeIdAndPriority != "C1")
            {
                return false;
            }
            pduStringInHex = pduStringInHex.Substring(4);
            var a = new CosemAttributeDescriptor();
            if (a.PduStringInHexConstructor(ref pduStringInHex))
            {
                AttributeDescriptor = a;
               
            }
            var b = pduStringInHex.Substring(0, 2);
           
            if (b == "00")
            {
                return true;
            }
            else if (b == "01")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                var aselestion = new SelectiveAccessDescriptor();
                if (aselestion.PduStringInHexConstructor(ref pduStringInHex))
                {
                    this.AccessSelection = aselestion;
                    this.AttributeDescriptorWithSelection = new CosemAttributeDescriptorWithSelection
                        (AttributeDescriptor, AccessSelection);
                    return true;
                }
                else { return false; }
            }
          

            return true;
        }
    }
}