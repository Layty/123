using System.Xml.Serialization;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums
{
    public enum Unit
    {
        [XmlEnum("0")]
        None = 0,
        [XmlEnum("1")]
        Year = 1,
        [XmlEnum("2")]
        Month = 2,
        [XmlEnum("3")]
        Week = 3,
        [XmlEnum("4")]
        Day = 4,
        [XmlEnum("5")]
        Hour = 5,
        [XmlEnum("6")]
        Minute = 6,
        [XmlEnum("7")]
        Second = 7,
        [XmlEnum("8")]
        PhaseAngleDegree = 8,
        [XmlEnum("9")]
        Temperature = 9,
        [XmlEnum("10")]
        LocalCurrency = 10,
        [XmlEnum("11")]
        Length = 11,
        [XmlEnum("12")]
        Speed = 12,
        [XmlEnum("13")]
        VolumeCubicMeter = 13,
        [XmlEnum("14")]
        CorrectedVolume = 14,
        [XmlEnum("15")]
        VolumeFluxHour = 0xF,
        [XmlEnum("16")]
        CorrectedVolumeFluxHour = 0x10,
        [XmlEnum("17")]
        VolumeFluxDay = 17,
        [XmlEnum("18")]
        CorrecteVolumeFluxDay = 18,
        [XmlEnum("19")]
        VolumeLiter = 19,
        [XmlEnum("20")]
        MassKg = 20,
        [XmlEnum("21")]
        Force = 21,
        [XmlEnum("22")]
        Energy = 22,
        [XmlEnum("23")]
        PressurePascal = 23,
        [XmlEnum("24")]
        PressureBar = 24,
        [XmlEnum("25")]
        EnergyJoule = 25,
        [XmlEnum("26")]
        ThermalPower = 26,
        [XmlEnum("27")]
        ActivePower = 27,
        [XmlEnum("28")]
        ApparentPower = 28,
        [XmlEnum("29")]
        ReactivePower = 29,
        [XmlEnum("30")]
        ActiveEnergy = 30,
        [XmlEnum("31")]
        ApparentEnergy = 0x1F,
        [XmlEnum("32")]
        ReactiveEnergy = 0x20,
        [XmlEnum("33")]
        Current = 33,
        [XmlEnum("34")]
        ElectricalCharge = 34,
        [XmlEnum("35")]
        Voltage = 35,
        [XmlEnum("36")]
        ElectricalFieldStrength = 36,
        [XmlEnum("37")]
        Capacity = 37,
        [XmlEnum("38")]
        Resistance = 38,
        [XmlEnum("39")]
        Resistivity = 39,
        [XmlEnum("40")]
        MagneticFlux = 40,
        [XmlEnum("41")]
        Induction = 41,
        [XmlEnum("42")]
        Magnetic = 42,
        [XmlEnum("43")]
        Inductivity = 43,
        [XmlEnum("44")]
        Frequency = 44,
        [XmlEnum("45")]
        Active = 45,
        [XmlEnum("46")]
        Reactive = 46,
        [XmlEnum("47")]
        Apparent = 47,
        [XmlEnum("48")]
        V260 = 48,
        [XmlEnum("49")]
        A260 = 49,
        [XmlEnum("50")]
        MassKgPerSecond = 50,
        [XmlEnum("51")]
        Conductance = 51,
        [XmlEnum("52")]
        Kelvin = 52,
        [XmlEnum("53")]
        RU2h = 53,
        [XmlEnum("54")]
        RI2h = 54,
        [XmlEnum("55")]
        CubicMeterRV = 55,
        [XmlEnum("56")]
        Percentage = 56,
        [XmlEnum("57")]
        AmpereHour = 57,
        [XmlEnum("60")]
        EnergyPerVolume = 60,
        [XmlEnum("61")]
        Wobbe = 61,
        [XmlEnum("62")]
        MolePercent = 62,
        [XmlEnum("63")]
        MassDensity = 0x3F,
        [XmlEnum("64")]
        PascalSecond = 0x40,
        [XmlEnum("65")]
        JouleKilogram = 65,
        [XmlEnum("70")]
        SignalStrength = 70,
        [XmlEnum("254")]
        OtherUnit = 254,
        [XmlEnum("255")]
        NoUnit = 0xFF
    }
}