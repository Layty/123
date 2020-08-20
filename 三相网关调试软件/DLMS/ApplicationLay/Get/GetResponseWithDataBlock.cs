using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetResponseWithDataBlock : IPduBytesToConstructor
    {
        public GetResponseType GetResponseType { get; set; } = GetResponseType.WithDataBlock;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
        public DataBlockG DataBlockG { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) GetResponseType)
            {
                return false;
            }

            InvokeIdAndPriority = new InvokeIdAndPriority();
            InvokeIdAndPriority.UpdateInvokeIdAndPriority(pduBytes[1]);
            InvokeIdAndPriority.Value = InvokeIdAndPriority.GetInvoke_Id_And_Priority();

            DataBlockG = new DataBlockG();

            return true;
        }
    }
}