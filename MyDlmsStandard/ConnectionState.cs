using System;

namespace MyDlmsStandard
{
    [Flags]
    public enum ConnectionState
    {
        None = 0x0,
        Hdlc = 0x1,
        Dlms = 0x2
    }
}