using System;

namespace MyDlmsStandard.ApplicationLay.ApplicationLayEnums
{
    [Flags]
    
    public enum Conformance : uint
    {
        ReservedZero = 0x800000,
        GeneralProtection = 0x400000,
        GeneralBlockTransfer = 0x200000,
        Read = 0x100000,
        Write = 0x80000,
        UnconfirmedWrite = 0x40000,
        ReservedSix = 0x20000,
        ReservedSeven = 0x10000,
        Attribute0SupportedWithSet = 0x8000,
        PriorityMgmtSupported = 0x4000,
        Attribute0SupportedWithGet = 0x2000,
        BlockTransferWithGetOrRead = 0x1000,
        BlockTransferWithSetOrWrite = 0x800,
        BlockTransferWithAction = 0x400,
        MultipleReferences = 0x200,
        InformationReport = 0x100,
        DataNotification = 0x80,
        Access = 0x40,
        ParameterizedAccess = 0x20,
        Get = 0x10,
        Set = 0x8,
        SelectiveAccess = 0x4,
        EventNotification = 0x2,
        Action = 0x1,
        None = 0x0
    }
}