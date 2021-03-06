namespace MyDlmsStandard.BusinessDefine
{
    public enum StandardEventCode
    {
        //标准事件代码
        停电 = 1,
        上电 = 2,
        时钟设置前 = 4,
        时钟设置后 = 5,
        时钟故障 = 6,
        时钟电池欠压 = 8,
        清理错误状态字 = 10,
        清理报警状态字 = 11,
        程序存储错误 = 12,
        RAM错误 = 13,
        电压逆相序 = 88,
        掉零线 = 89,
        电表清零 = 253,
        需量清零 = 254,

        //窃电监测事件代码
        开端盖 = 40,
        合端盖 = 41,
        开表盖 = 44,
        合表盖 = 45,
        电流逆相序 = 91,
    }
}