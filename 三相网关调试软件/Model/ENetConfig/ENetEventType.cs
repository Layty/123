namespace 三相智慧能源网关调试软件.Model.ENetConfig
{

    public enum ENetEventType
    {
        SortVersion = 0,
        基表运行模式 = 1,
        运行状态字 = 2,
        错误状态字 = 3,
        窃电状态字 = 4,
        计量状态字 = 5,
        通讯状态字 = 6,
        基表校时参数与标志 = 7,
        遥信及温湿度 = 8,
        频率及谐波含量 = 9,
        当前电量 = 10,
        上一月电量 = 11,
        上两月电量 = 12,
        电压电流功率 = 13,
        当前需量 = 14,
        上一月需量 = 15,
        上二月需量 = 16,
        系统日志 = 22,
        用户日志 = 23,
        设置传感器档案参数 = 24,
        获取传感器实时数据带参数 = 25,
        Set配置参数模块 = 26,
        设备列表信息 = 27,
        读取传感器档案参数 = 28,
        获取网关系统参数 = 29,
        获取端表盖状态 = 32,
        获取Yx遥信状态 = 33,
        获取IIC状态电池状态 = 34
    }
}