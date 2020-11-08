namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    /// <summary>
    /// 接口类base本身没有明确规定，它只包含一个属性“逻辑名”
    /// </summary>
    public abstract class CosemBase
    {
        public string LogicalName { get; set; }
    }
}