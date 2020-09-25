using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class SelectiveAccessDescriptor : IToPduBytes,IPduStringInHexConstructor
    {
        public AxdrUnsigned8 AccessSelector { get; set; }
        public AccessParameters AccessParameters { get; set; }

        public SelectiveAccessDescriptor()
        {
            
        }
        public SelectiveAccessDescriptor(AxdrUnsigned8 accessSelector , DLMSDataItem dlmsDataItem)
        {
            AccessSelector = accessSelector;
            AccessParameters=new AccessParameters(){Data = dlmsDataItem };
        }
        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add(AccessSelector.GetEntityValue());
            list.AddRange(AccessParameters.ToPduBytes());
            return list.ToArray();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            AccessSelector = new AxdrUnsigned8();
            if (!AccessSelector.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            AccessParameters = new AccessParameters();
            if (!AccessParameters.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
        }
    }
}