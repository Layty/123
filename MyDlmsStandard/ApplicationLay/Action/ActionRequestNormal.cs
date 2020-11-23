using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Action
{
    public class ActionRequestNormal :IToPduStringInHex
    {
        [XmlIgnore] protected ActionRequestType ActionRequestType { get; set; } = ActionRequestType.Normal;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
    
        public AxdrIntegerUnsigned8 InvokeIdAndPriority1 { get; set; }
        public CosemMethodDescriptor CosemMethodDescriptor { get; set; }

        public DlmsDataItem MethodInvocationParameters { get; set; }

        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem methodInvocationParameters)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            MethodInvocationParameters = methodInvocationParameters;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);

            InvokeIdAndPriority1=new AxdrIntegerUnsigned8("C1");
        }

        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);

            InvokeIdAndPriority1 = new AxdrIntegerUnsigned8("C1");
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


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(InvokeIdAndPriority1.ToPduStringInHex());
            stringBuilder.Append(CosemMethodDescriptor.ToPduStringInHex());
            if (MethodInvocationParameters != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(MethodInvocationParameters.ToPduStringInHex());
            }
            else
            {
                stringBuilder.Append("00");
            }
            return stringBuilder.ToString();
        }
    }
}