namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums
{
    public enum Priority
    {
        Normal,
        High
    }

    public enum RequestTypes
    {
        None = 0,
        DataBlock = 1,
        Frame = 2,
        GBT = 4
    }
    public enum Security
    {
        None = 0,
        Authentication = 0x10,
        Encryption = 0x20,
        AuthenticationEncryption = 48
    }
}
