﻿using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class GetRequestNext : IToPduBytes
    {
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
            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
            var blockNumberBytes = BitConverter.GetBytes(BlockNumber).Reverse().ToArray();
            pduBytes.AddRange(blockNumberBytes);
            return pduBytes.ToArray();
        }
    }
}