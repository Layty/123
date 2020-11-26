using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay
{
    public class CosemAttributeDescriptor : IPduStringInHexConstructor, IToPduStringInHex
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
        /// 属性ID
        /// </summary>
        public AxdrIntegerInteger8 AttributeId { get; set; }

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
            if (AttributeId != null)
            {
                num += AttributeId.Length;
            }
            return num;
        }

        public CosemAttributeDescriptor()
        {
        }

        public CosemAttributeDescriptor(AxdrIntegerUnsigned16 classId, AxdrOctetStringFixed instanceId, AxdrIntegerInteger8 attributeId)
        {
            ClassId = classId;
            InstanceId = instanceId;
            AttributeId = attributeId;
        }
   


        public string ToPduStringInHex()
        {
            return ClassId.ToPduStringInHex() + InstanceId.ToPduStringInHex() +
                   AttributeId.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            AxdrIntegerUnsigned16 cosemClassId = new AxdrIntegerUnsigned16();
            if (!cosemClassId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }


            AxdrOctetStringFixed cosemObjectInstanceId = new AxdrOctetStringFixed(6);
            if (!cosemObjectInstanceId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            AxdrIntegerInteger8 cosemObjectAttributeId = new AxdrIntegerInteger8();
            if (!cosemObjectAttributeId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}