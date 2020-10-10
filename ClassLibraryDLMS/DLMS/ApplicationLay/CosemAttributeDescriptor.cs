using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay
{
    public class CosemAttributeDescriptor : IPduStringInHexConstructor, IToPduStringInHex
    {
        public AxdrUnsigned16 ClassId { get; set; }
        public AxdrOctetStringFixed InstanceId { get; set; }
        public AxdrInteger8 AttributeId { get; set; }

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

        public CosemAttributeDescriptor(AxdrUnsigned16 classId, AxdrOctetStringFixed instanceId, AxdrInteger8 attributeId)
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