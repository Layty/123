using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.OBIS;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class AttributeDescriptor : IToPduBytes,IPduStringInHexConstructor
    {
        public ObjectType ClassId { get; set; }
        public string InstanceId { get; set; }
        public byte AttributeId { get; set; }

        public int Length => CalculateLength();

        private int CalculateLength()
        {
            return 10;
        }

        public AttributeDescriptor()
        {
        }

        public AttributeDescriptor(CosemObject cosemObject, byte index)
        {
            ClassId = cosemObject.ObjectType;
            InstanceId = cosemObject.LogicalName;
            AttributeId = index;
        }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes((ushort) ClassId).Reverse()); //ClassId
            list.AddRange(ObisHelper.ObisStringToBytes(InstanceId));
            list.Add(AttributeId); //方法
            return list.ToArray();
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