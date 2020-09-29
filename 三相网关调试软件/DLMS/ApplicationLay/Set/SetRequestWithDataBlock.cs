using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetRequestWithDataBlock:IToPduBytes
    {
        protected SetRequestType SetRequestType { get; set; } = SetRequestType.WithDataBlock;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }

        public byte[] ToPduBytes()
        {
            throw new System.NotImplementedException();
        }

        
    }
}