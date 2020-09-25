using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.OBIS;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{

    public class CosemMethodDescriptor : IToPduBytes
    {
        public ObjectType ClassId { get; set; }
        public string InstanceId { get; set; }
        public byte MethodId { get; set; }
        
        public int Length => CalculateLength();
        private int CalculateLength()
        {
            return 10;
        }
        public CosemMethodDescriptor(CosemObject cosemObject, byte index)
        {
            ClassId = cosemObject.ObjectType;
            InstanceId = cosemObject.LogicalName;
            MethodId = index;
        }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes((ushort) ClassId).Reverse()); //ClassId
            list.AddRange(ObisHelper.ObisStringToBytes(InstanceId));
            list.Add(MethodId); //方法
            return list.ToArray();
        }
    }
}