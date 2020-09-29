using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class CosemAttributeDescriptor : IPduStringInHexConstructor, IToPduStringInHex
    {
        public AxdrUnsigned16 CosemClassId { get; set; }
        public AxdrOctetStringFixed CosemObjectInstanceId { get; set; }
        public AxdrInteger8 CosemObjectAttributeId { get; set; }

        public int Length => CalculateLength();

        private int CalculateLength()
        {
            int num = 0;
            if (CosemClassId != null)
            {
                num += CosemClassId.Length;
            }
            if (CosemObjectInstanceId != null)
            {
                num += CosemObjectInstanceId.Length;
            }
            if (CosemObjectAttributeId != null)
            {
                num += CosemObjectAttributeId.Length;
            }
            return num;
        }

        public CosemAttributeDescriptor()
        {
        }

        public CosemAttributeDescriptor(AxdrUnsigned16 cosemClassId, AxdrOctetStringFixed cosemObjectInstanceId, AxdrInteger8 cosemObjectAttributeId)
        {
            CosemClassId = cosemClassId;
            CosemObjectInstanceId = cosemObjectInstanceId;
            CosemObjectAttributeId = cosemObjectAttributeId;
        }
   


        public string ToPduStringInHex()
        {
            return CosemClassId.ToPduStringInHex() + CosemObjectInstanceId.ToPduStringInHex() +
                   CosemObjectAttributeId.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            AxdrUnsigned16 cosemClassId = new AxdrUnsigned16();
            if (!cosemClassId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }


            AxdrOctetStringFixed cosemObjectInstanceId = new AxdrOctetStringFixed(6);
            if (!cosemObjectInstanceId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            AxdrInteger8 cosemObjectAttributeId = new AxdrInteger8();
            if (!cosemObjectAttributeId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}