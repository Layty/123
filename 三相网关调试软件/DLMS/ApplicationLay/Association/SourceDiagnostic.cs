namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association
{
    public enum SourceDiagnostic
    {
        None,
        NoReasonGiven,
        ApplicationContextNameNotSupported,
        CallingApTitleNotRecognized,
        CallingApInvocationIdentifierNotRecognized,
        CallingAeQualifierNotRecognized,
        CallingAeInvocationIdentifierNotRecognized,
        CalledApTitleNotRecognized,
        CalledApInvocationIdentifierNotRecognized,
        CalledAeQualifierNotRecognized,
        CalledAeInvocationIdentifierNotRecognized,
        AuthenticationMechanismNameNotRecognised,
        AuthenticationMechanismNameReguired,
        AuthenticationFailure,
        AuthenticationRequired
    }
}