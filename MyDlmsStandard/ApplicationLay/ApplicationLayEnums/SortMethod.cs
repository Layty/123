namespace MyDlmsStandard.ApplicationLay.ApplicationLayEnums
{/// <summary>
/// 曲线的排序
/// </summary>
    public enum SortMethod
    {
        None = 0,
        FiFo = 1,//先进先出
        LiFo,//后进先出
        Largest,
        Smallest,
        NearestToZero,
        FarestFromZero
    }
}