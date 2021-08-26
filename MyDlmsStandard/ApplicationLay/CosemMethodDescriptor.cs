using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay
{
    /// <summary>
    /// Cosem方法描述
    /// </summary>
    public class CosemMethodDescriptor : IToPduStringInHex, IPduStringInHexConstructor
    {
        /// <summary>
        /// 类ID
        /// </summary>
        public AxdrIntegerUnsigned16 ClassId { get; set; }

        /// <summary>
        /// OBIS
        /// </summary>
        public AxdrOctetStringFixed InstanceId { get; set; }

        /// <summary>
        /// 方法ID
        /// </summary>
        public AxdrInteger8 CosemObjectMethodId { get; set; }

        public int Length => CalculateLength();

        private int CalculateLength()
        {
            int num = 0;
            if (ClassId != null)
            {
                num += ClassId.Length;
            }

            if (InstanceId != null)
            {
                num += InstanceId.Length;
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

        public CosemMethodDescriptor(AxdrIntegerUnsigned16 classId, AxdrOctetStringFixed instanceId,
            AxdrInteger8 cosemObjectMethodId)
        {
            ClassId = classId;
            InstanceId = instanceId;
            CosemObjectMethodId = cosemObjectMethodId;
        }

        public string ToPduStringInHex()
        {
            return ClassId.ToPduStringInHex() + InstanceId.ToPduStringInHex() +
                   CosemObjectMethodId.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            ClassId = new AxdrIntegerUnsigned16();
            if (!ClassId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            InstanceId = new AxdrOctetStringFixed(6);
            if (!InstanceId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            CosemObjectMethodId = new AxdrInteger8();
            if (!CosemObjectMethodId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}