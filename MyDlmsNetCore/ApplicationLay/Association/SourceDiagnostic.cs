namespace MyDlmsNetCore.ApplicationLay.Association
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