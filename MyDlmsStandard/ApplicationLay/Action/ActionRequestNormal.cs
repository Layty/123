using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Action
{
    public class ActionRequestNormal : IToPduStringInHex
    {
        [XmlIgnore] protected ActionRequestType ActionRequestType { get; set; } = ActionRequestType.Normal;


        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public CosemMethodDescriptor CosemMethodDescriptor { get; set; }

        public DlmsDataItem MethodInvocationParameters { get; set; }

        public ActionRequestNormal()
        {
            CosemMethodDescriptor = new CosemMethodDescriptor();
            MethodInvocationParameters = new DlmsDataItem();
            InvokeIdAndPriority = new AxdrIntegerUnsigned8("C1");
        }
        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem methodInvocationParameters)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            MethodInvocationParameters = methodInvocationParameters;
            InvokeIdAndPriority = new AxdrIntegerUnsigned8("C1");
        }

        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            InvokeIdAndPriority = new AxdrIntegerUnsigned8("C1");
        }



        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("01");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
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