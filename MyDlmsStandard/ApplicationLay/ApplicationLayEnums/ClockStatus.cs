using System;

namespace MyDlmsStandard.ApplicationLay.ApplicationLayEnums
{
 

    [Flags]
    public enum ClockStatus
    {
        Ok = 0x0,
        InvalidValue = 0x1,
        DoubtfulValue = 0x2,
        DifferentClockBase = 0x4,
        InvalidClockStatus = 0x8,
        DaylightSavingActive = 0x80,
        Skip = 0xFF
    }
}