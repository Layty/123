using System;

namespace 三相智慧能源网关调试软件.DLMS
{
    [Flags]
    public enum ConnectionState
    {
        None = 0x0,
        Hdlc = 0x1,
        Dlms = 0x2
    }
}