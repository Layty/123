namespace DLMSMaster.ApplicationLay.Enums
{
    internal enum PduType
    {
        ProtocolVersion = 0,
        ApplicationContextName = 1,
        CalledApTitle = 2,
        CalledAeQualifier = 3,
        CalledApInvocationId = 4,
        CalledAeInvocationId = 5,
        CallingApTitle = 6,
        CallingAeQualifier = 7,
        CallingApInvocationId = 8,
        CallingAeInvocationId = 9,
        SenderAcseRequirements = 10,
        MechanismName = 11,
        CallingAuthenticationValue = 12,
        ImplementationInformation = 29,
        UserInformation = 30
    }
}