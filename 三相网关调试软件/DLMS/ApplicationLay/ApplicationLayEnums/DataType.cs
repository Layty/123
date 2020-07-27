namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums
{
    public enum DataType : byte
    {
        NullData=0,
        Array = 1,
        Structure = 2,
        Boolean = 3,
        BitString = 4,
        Int32 = 5,
        UInt32 = 6,

        double32 = 7,
        double64 = 8,
        
        OctetString = 9,
        String = 10,
        UTF8String = 12,
        Bcd = 13,
        Int8 = 15,
        Int16 = 16, UInt8 = 17, UInt16 = 18,
        CompactArray = 19,
        Int64 = 20,
        UInt64 = 21,
        Enum = 22,
        Float32 = 23,
        Float64 = 24,
        DateTime = 25,
        Date = 26,
        Time = 27,
        DoNotCare=255


    


    


  

    }
}