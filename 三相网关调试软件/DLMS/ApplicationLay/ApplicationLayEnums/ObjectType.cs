using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums
{
    public enum ObjectType:ushort
    {
        [XmlEnum("0")]
        None = 0,
        [XmlEnum("22")]
        ActionSchedule = 22,
        [XmlEnum("20")]
        ActivityCalendar = 20,
        [XmlEnum("15")]
        AssociationLogicalName = 0xF,
        [XmlEnum("12")]
        AssociationShortName = 12,
        [XmlEnum("28")]
        AutoAnswer = 28,
        [XmlEnum("29")]
        AutoConnect = 29,
        [XmlEnum("8")]
        Clock = 8,
        [XmlEnum("1")]
        Data = 1,
        [XmlEnum("5")]
        DemandRegister = 5,
        [XmlEnum("43")]
        MacAddressSetup = 43,
        [XmlEnum("4")]
        ExtendedRegister = 4,
        [XmlEnum("45")]
        GprsSetup = 45,
        [XmlEnum("23")]
        IecHdlcSetup = 23,
        [XmlEnum("19")]
        IecLocalPortSetup = 19,
        [XmlEnum("24")]
        IecTwistedPairSetup = 24,
        [XmlEnum("42")]
        Ip4Setup = 42,
        [XmlEnum("47")]
        GSMDiagnostic = 47,
        [XmlEnum("48")]
        Ip6Setup = 48,
        [XmlEnum("25")]
        MBusSlavePortSetup = 25,
        [XmlEnum("27")]
        ModemConfiguration = 27,
        [XmlEnum("40")]
        PushSetup = 40,
        [XmlEnum("44")]
        PppSetup = 44,
        [XmlEnum("7")]
        ProfileGeneric = 7,
        [XmlEnum("3")]
        Register = 3,
        [XmlEnum("6")]
        RegisterActivation = 6,
        [XmlEnum("21")]
        RegisterMonitor = 21,
        [XmlEnum("70")]
        DisconnectControl = 70,
        [XmlEnum("71")]
        Limiter = 71,
        [XmlEnum("72")]
        MBusClient = 72,
        [XmlEnum("62")]
        CompactData = 62,
        [XmlEnum("65")]
        ParameterMonitor = 65,
        [XmlEnum("73")]
        WirelessModeQchannel = 73,
        [XmlEnum("74")]
        MBusMasterPortSetup = 74,
        [XmlEnum("80")]
        LlcSscsSetup = 80,
        [XmlEnum("81")]
        PrimeNbOfdmPlcPhysicalLayerCounters = 81,
        [XmlEnum("82")]
        PrimeNbOfdmPlcMacSetup = 82,
        [XmlEnum("83")]
        PrimeNbOfdmPlcMacFunctionalParameters = 83,
        [XmlEnum("84")]
        PrimeNbOfdmPlcMacCounters = 84,
        [XmlEnum("85")]
        PrimeNbOfdmPlcMacNetworkAdministrationData = 85,
        [XmlEnum("86")]
        PrimeNbOfdmPlcApplicationsIdentification = 86,
        [XmlEnum("61")]
        RegisterTable = 61,
        [XmlEnum("101")]
        ZigBeeSasStartup = 101,
        [XmlEnum("102")]
        ZigBeeSasJoin = 102,
        [XmlEnum("103")]
        ZigBeeSasApsFragmentation = 103,
        [XmlEnum("104")]
        ZigBeeNetworkControl = 104,
        [XmlEnum("30")]
        DataProtection = 30,
        [XmlEnum("111")]
        Account = 111,
        [XmlEnum("112")]
        Credit = 112,
        [XmlEnum("113")]
        Charge = 113,
        [XmlEnum("115")]
        TokenGateway = 115,
        [XmlEnum("17")]
        SapAssignment = 17,
        [XmlEnum("18")]
        ImageTransfer = 18,
        [XmlEnum("10")]
        Schedule = 10,
        [XmlEnum("9")]
        ScriptTable = 9,
        [XmlEnum("2")]
        SmtpSetup = 2,
        [XmlEnum("11")]
        SpecialDaysTable = 11,
        [XmlEnum("63")]
        StatusMapping = 0x3F,
        [XmlEnum("64")]
        SecuritySetup = 0x40,
        [XmlEnum("41")]
        TcpUdpSetup = 41,
        [XmlEnum("26")]
        UtilityTables = 26,
        [XmlEnum("115")]
        Token = 115,
        [XmlEnum("50")]
        SFSKPhyMacSetUp = 50,
        [XmlEnum("51")]
        SFSKActiveInitiator = 51,
        [XmlEnum("52")]
        SFSKMacSynchronizationTimeouts = 52,
        [XmlEnum("53")]
        SFSKMacCounters = 53,
        [XmlEnum("90")]
        G3PlcMacLayerCounters = 90,
        [XmlEnum("91")]
        G3PlcMacSetup = 91,
        [XmlEnum("92")]
        G3Plc6LoWPan = 92,
        [XmlEnum("150")]
        IEC14908Identification = 150,
        [XmlEnum("151")]
        IEC14908PhysicalSetup = 151,
        [XmlEnum("152")]
        IEC14908PhysicalStatus = 152,
        [XmlEnum("153")]
        IEC14908Diagnostic = 153,
        [XmlEnum("8192")]
        TariffPlan = 0x2000
    }
}
