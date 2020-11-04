﻿using System.Collections.Generic;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Action
{
    public class ActionRequestNormal :  IToPduBytes
    {
        [XmlIgnore] protected ActionRequestType ActionRequestType { get; set; } = ActionRequestType.Normal;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
    
        public CosemMethodDescriptor CosemMethodDescriptor { get; set; }

        public DLMSDataItem MethodInvocationParameters { get; set; }

        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor,
            DLMSDataItem methodInvocationParameters)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            MethodInvocationParameters = methodInvocationParameters;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            
            pduBytes.Add((byte) ActionRequestType);
            pduBytes.Add(InvokeIdAndPriority.Value);
            if (CosemMethodDescriptor != null)
            {
                pduBytes.AddRange(CosemMethodDescriptor.ToPduStringInHex().StringToByte());
            }

            if (MethodInvocationParameters != null)
            {
                pduBytes.Add(0x01);
                pduBytes.AddRange( (MethodInvocationParameters.ToPduBytes()));
            }
            else
            {
                pduBytes.Add(0x00);
            }

            return pduBytes.ToArray();
        }

      
    }
}