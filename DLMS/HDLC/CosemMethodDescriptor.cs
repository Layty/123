using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.OBIS;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public struct CosemMethodDescriptor : IToPduBytes
    {
        public ObjectType ClassId { get; set; }
        public string InstanceId { get; set; }
        public byte MethodId { get; set; }
        public byte Version { get; set; }

        public int Length => CalculateLength();
        private int CalculateLength()
        {
            return 10;
        }
        public CosemMethodDescriptor(DLMSObject dlmsObject, byte index)
        {
            ClassId = dlmsObject.ObjectType;
            InstanceId = dlmsObject.LogicalName;
            MethodId = index;
            Version = dlmsObject.Version;
        }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes((ushort) ClassId).Reverse()); //ClassId
            list.AddRange(OBISHelper.ObisStringToBytes(InstanceId));
            list.Add(MethodId); //方法
            list.Add(Version); //版本 
            return list.ToArray();
        }
    }
}