using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetResponseWithList : IPduBytesToConstructor
    {
        public Command Command { get; set; } = Command.GetResponse;
        public GetResponseType GetResponseType { get; set; } = GetResponseType.WithList;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }

        public GetDataResult[] Result;

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) Command)
            {
                return false;
            }

            if (pduBytes[1] != (byte) GetResponseType)
            {
                return false;
            }

            InvokeIdAndPriority = new InvokeIdAndPriority(pduBytes[2]);
            InvokeIdAndPriority.Value = InvokeIdAndPriority.GetInvoke_Id_And_Priority();

            Result = new GetDataResult[] { };
            return true;
        }
    }
}