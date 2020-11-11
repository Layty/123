using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class CosemMethodDescriptor : IToPduStringInHex, IPduStringInHexConstructor
    {
        public AxdrIntegerUnsigned16 CosemClassId { get; set; }

        public AxdrOctetStringFixed CosemObjectInstanceId { get; set; }
        public AxdrIntegerInteger8 CosemObjectMethodId { get; set; }

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

            if (CosemObjectMethodId != null)
            {
                num += CosemObjectMethodId.Length;
            }

            return num;
        }

        public CosemMethodDescriptor()
        {
        }

        public CosemMethodDescriptor(AxdrIntegerUnsigned16 cosemClassId, AxdrOctetStringFixed cosemObjectInstanceId,
            AxdrIntegerInteger8 cosemObjectMethodId)
        {
            CosemClassId = cosemClassId;
            CosemObjectInstanceId = cosemObjectInstanceId;
            CosemObjectMethodId = cosemObjectMethodId;
        }

        public string ToPduStringInHex()
        {
            return CosemClassId.ToPduStringInHex() + CosemObjectInstanceId.ToPduStringInHex() +
                   CosemObjectMethodId.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            CosemClassId = new AxdrIntegerUnsigned16();
            if (!CosemClassId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            CosemObjectInstanceId = new AxdrOctetStringFixed(6);
            if (!CosemObjectInstanceId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            CosemObjectMethodId = new AxdrIntegerInteger8();
            if (!CosemObjectMethodId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}