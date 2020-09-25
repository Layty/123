namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums
{
    public enum DataAccessResult
    {
        Success = 0,
        HardwareFault = 1,
        TemporaryFailure = 2,
        ReadWriteDenied = 3,
        ObjectUndefined = 4,
        ObjectClassInconsistent = 9,
        ObjectUnavailable = 11,
        TypeUnmatched = 12,
        ScopeOfAccessViolated = 13,
        DataBlockUnavailable = 14,
        LongGetAborted = 15,
        NoLongGetInProgress = 16,
        LongSetAborted = 17,
        NoLongSetInProgress = 18,
        DataBlockNumberInvalid = 19,
        OtherReason = 250
    }
}