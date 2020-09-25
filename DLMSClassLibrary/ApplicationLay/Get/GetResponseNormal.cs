using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class ExceptionResponse : IPduBytesToConstructor
    {
        [XmlIgnore] public Command Command { get; set; } = Command.ExceptionResponse;

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) Command)
            {
                return false;
            }

            return
                true;
            
        }
    }

    public class Response 
    {
        public Command Command { get; set; }
        public GetResponse GetResponse { get; set; }
        public ExceptionResponse ExceptionResponse { get; set; }

    }


    public class GetResponseNormal : IToPduStringInHex,IPduStringInHexConstructor
    {
        [XmlIgnore] public GetResponseType GetResponseType { get; set; } = GetResponseType.Normal;
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }

        public GetDataResult Result { get; set; }

        public string ToPduStringInHex()
        {
            return InvokeIdAndPriority.ToPduStringInHex() +Result.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
          
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

         
          
            InvokeIdAndPriority = new AxdrUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            Result = new GetDataResult();
            if (!Result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
        }
    }
}