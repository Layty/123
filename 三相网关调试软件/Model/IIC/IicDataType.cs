namespace 三相智慧能源网关调试软件.Model.IIC
{
    public enum IicDataType
    {
        IicInstantData = 0x8001,
        IicCurrentEnergyData = 0x8010,
        IicLast1EnergyData = 0x8011,
        IicLast2EnergyData = 0x8012,
        IicCurrentDemandData = 0x8020,
        IicLast1DemandData = 0x8021,
        IicLast2DemandData = 0x8022,
        IicHarmonicData = 0x8030,
        IicParameterData = 0x8100

    }
}