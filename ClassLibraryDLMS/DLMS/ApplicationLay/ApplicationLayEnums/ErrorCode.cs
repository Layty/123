namespace ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums
{
    public enum ErrorCode
    {
        DisconnectMode = -4,
        ReceiveNotReady = -3,
        Rejected = -2,
        UnacceptableFrame = -1,
        Ok = 0,
        HardwareFault = 1,
        TemporaryFailure = 2,
        ReadWriteDenied = 3,
        UndefinedObject = 4,
        InconsistentClass = 9,
        UnavailableObject = 11,
        UnmatchedType = 12,
        AccessViolated = 13,
        DataBlockUnavailable = 14,
        LongGetOrReadAborted = 0xF,
        NoLongGetOrReadInProgress = 0x10,
        LongSetOrWriteAborted = 17,
        NoLongSetOrWriteInProgress = 18,
        DataBlockNumberInvalid = 19,
        OtherReason = 250
    }
}
