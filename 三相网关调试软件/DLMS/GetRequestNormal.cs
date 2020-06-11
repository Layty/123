using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class DataBlockG
    {
        public bool IsLastBlock { get; set; }
        public uint BlockNumber { get; set; }

        public string RawData { get; set; }
        public byte DataAccessResult { get; set; }
    }

    public class GetRequestNormal : IToPduBytes
    {
        protected Command Command { get; set; } = Command.GetRequest;
        protected GetRequestType GetRequestType { get; set; }
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        public CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }

        public GetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            GetRequestType = getRequestType;
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) Command);
            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
            if (CosemAttributeDescriptor != null)
            {
                pduBytes.AddRange(CosemAttributeDescriptor.ToPduBytes());
            }

            return pduBytes.ToArray();
        }
    }

    public class GetRequestNext : IToPduBytes
    {
        protected Command Command { get; set; } = Command.GetRequest;
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.Next;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        public uint BlockNumber { get; set; } = 0;

        public GetRequestNext()
        {
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) Command);
            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
            var blockNumberBytes = BitConverter.GetBytes(BlockNumber).Reverse().ToArray();
            pduBytes.AddRange(blockNumberBytes);
            return pduBytes.ToArray();
        }
    }

    public class GetRequestWithList : IToPduBytes
    {
        protected Command Command { get; set; } = Command.GetRequest;
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.WithList;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }

        public CosemAttributeDescriptor[] CosemAttributeDescriptors { get; set; }
        public GetRequestWithList()
        {
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte)Command);
            pduBytes.Add((byte)GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
           
           
            return pduBytes.ToArray();
        }
    }
}