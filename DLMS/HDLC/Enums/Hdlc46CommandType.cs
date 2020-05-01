using 三相智慧能源网关调试软件.Commom;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public enum Hdlc46CommandType
    {
        InfoFrame = 16,
        ReceiveReady = 0,
        ReceiveNotReady = 21,
        UiCommand = 19,
        SNRMCommand = 147,
        DiscCommand = 83
    }
}