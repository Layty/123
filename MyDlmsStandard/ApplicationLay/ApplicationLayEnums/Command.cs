namespace MyDlmsStandard.ApplicationLay.ApplicationLayEnums
{
    public enum Command
    {
        None = 0,
        InitiateRequest = 1,
        InitiateResponse = 8,
        ReadRequest = 5,
        ReadResponse = 12,
        WriteRequest = 6,
        WriteResponse = 13,
        GetRequest = 192,
        GetResponse = 196,
        SetRequest = 193,
        SetResponse = 197,
        MethodRequest = 195,
        MethodResponse = 199,
        DisconnectMode = 0x1F,
        UnacceptableFrame = 151,
        Snrm = 147,
        Ua = 115,
        UiCommand = 19,
        Aarq = 96,
        Aare = 97,
        DisconnectRequest = 83,
        ReleaseRequest = 98,
        ReleaseResponse = 99,
        ConfirmedServiceError = 14,
        ExceptionResponse = 216,
        GeneralBlockTransfer = 224,
        AccessRequest = 217,
        AccessResponse = 218,
        DataNotification = 0x0F,
        GloGetRequest = 200,
        GloGetResponse = 204,
        GloSetRequest = 201,
        GloSetResponse = 205,
        GloEventNotification = 202,
        GloMethodRequest = 203,
        GloMethodResponse = 207,
        GloInitiateRequest = 33,
        GloReadRequest = 37,
        GloWriteRequest = 38,
        GloInitiateResponse = 40,
        GloReadResponse = 44,
        GloWriteResponse = 45,
        GeneralGloCiphering = 219,
        GeneralDedCiphering = 220,
        GeneralCiphering = 221,
        InformationReport = 24,
        EventNotification = 194,
        DedInitiateRequest = 65,
        DedReadRequest = 69,
        DedWriteRequest = 70,
        DedInitiateResponse = 72,
        DedReadResponse = 76,
        DedWriteResponse = 77,
        DedConfirmedServiceError = 78,
        DedUnconfirmedWriteRequest = 86,
        DedInformationReport = 88,
        DedGetRequest = 208,
        DedGetResponse = 212,
        DedSetRequest = 209,
        DedSetResponse = 213,
        DedEventNotification = 210,
        DedMethodRequest = 211,
        DedMethodResponse = 215,
        GatewayRequest = 230,
        GatewayResponse = 231
    }
}