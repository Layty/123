using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class GetDataResult
    {
        public ErrorCode DataAccessResult { get; set; }
        public DLMSDataItem Data { get; set; }
    }
}