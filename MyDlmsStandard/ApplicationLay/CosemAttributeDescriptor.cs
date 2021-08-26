using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay
{
    /// <summary>
    /// Cosem属性描述
    /// </summary>
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
        public AxdrInteger8 AttributeId { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
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

        public CosemAttributeDescriptor(AxdrIntegerUnsigned16 classId, AxdrOctetStringFixed instanceId,
            AxdrInteger8 attributeId)
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

            AttributeId = new AxdrInteger8();
            if (!AttributeId.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}