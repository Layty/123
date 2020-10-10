namespace ClassLibraryDLMS.DLMS.OBIS
{
    public abstract class AbstractObisClass
    {
        /// <summary>
        /// 0:抽象对象，1:与电有关的对象 ，其他
        /// </summary>
        private byte A { get; set; }
        /// <summary>
        /// //0:无指定通道，1:通道1 ，其他
        /// </summary>
        private byte B { get; set; }
        /// <summary>
        /// A=0 抽象对象; A=1  与电有关的对象 如电压电流等等
        /// </summary>
        private byte C { get; set; }
        /// <summary>
        /// //A=1,C<>不等于 0,96,97,98,99  如最大值最小值平均值等等
        /// </summary>
        private byte D { get; set; }
        /// <summary>
        /// //与 电能 有关的对象 A=1,  总，费率1,2,3,4..
        /// </summary>
        private byte E { get; set; }
        /// <summary>
        /// 如何情况下如果数值组F 没有被使用，则他被设置为255
        /// 用于计费周期
        /// </summary>
        private byte F { get; set; }
    }
}