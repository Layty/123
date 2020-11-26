namespace MyDlmsNetCore.ApplicationLay.ApplicationLayEnums
{
    internal enum BerType
    {
        EOC = 0,
        Boolean = 1,
        Integer = 2,
        BitString = 3,
        OctetString = 4,
        Null = 5,
        ObjectIdentifier = 6,
        ObjectDescriptor = 7,
        External = 8,
        Real = 9,
        Enumerated = 10,
        Utf8StringTag = 12,
        NumericString = 18,
        PrintableString = 19,
        TeletexString = 20,
        VideotexString = 21,
        Ia5String = 22,
        UtcTime = 23,
        GeneralizedTime = 24,
        GraphicString = 25,
        VisibleString = 26,
        GeneralString = 27,
        UniversalString = 28,
        BmpString = 30,
        Application = 0x40,
        Context = 0x80,
        Private = 192,
        Constructed = 0x20
    }
}