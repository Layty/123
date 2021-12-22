using MyDlmsStandard.ApplicationLay.CosemObjects.ProfileGeneric;
using System.Collections.ObjectModel;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 对曲线的OBIS的定义
    /// </summary>
    public static class ProfileGenericLogicNameDefine
    {
        public const string 一分钟电量曲线 = "1.0.99.1.0.255";
        public const string 十五分钟功率曲线 = "1.0.99.2.0.255";
        public const string 日冻结电量曲线 = "1.0.98.1.1.255";
        public const string 月冻结电量曲线 = "0.0.98.1.0.255";

    }
    /// <summary>
    /// 曲线类默认的捕获对象集合定义
    /// </summary>
    public static class ProfileGenericDefalutCaptrueObject
    {
        /// <summary>
        /// 分钟电量曲线的默认捕获对象集合
        /// </summary>
        public static ObservableCollection<CaptureObjectDefinition> Energy = new ObservableCollection<CaptureObjectDefinition>()
        {
            new CaptureObjectDefinition(){ ClassId=8,LogicalName="0.0.1.0.0.255",AttributeIndex=2,DataIndex=0,Description="Clock time"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.0.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.1.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能尖"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.2.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能峰"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.3.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能平"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.4.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能谷"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.2.8.0.255",AttributeIndex=2,DataIndex=0,Description="反向有功总电能"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.3.8.0.255",AttributeIndex=2,DataIndex=0,Description="正向无功总电能"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.4.8.0.255",AttributeIndex=2,DataIndex=0,Description="反向无功总电能"},
        };
        /// <summary>
        /// 功率负荷曲线的默认捕获对象集合
        /// </summary>
        public static ObservableCollection<CaptureObjectDefinition> Power = new ObservableCollection<CaptureObjectDefinition>()
        {
              new CaptureObjectDefinition(){ ClassId=8,LogicalName="0.0.1.0.0.255",AttributeIndex=2,DataIndex=0,Description="Clock time"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.7.0.255",AttributeIndex=2,DataIndex=0,Description="总正向有功功率"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.2.7.0.255",AttributeIndex=2,DataIndex=0,Description="总反向有功功率"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.32.7.0.255",AttributeIndex=2,DataIndex=0,Description="L1 相电压"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.52.7.0.255",AttributeIndex=2,DataIndex=0,Description="L2 相电压"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.72.7.0.255",AttributeIndex=2,DataIndex=0,Description="L3 相电压"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.31.7.0.255",AttributeIndex=2,DataIndex=0,Description="L1 相电流"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.51.7.0.255",AttributeIndex=2,DataIndex=0,Description="L2 相电流"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.71.7.0.255",AttributeIndex=2,DataIndex=0,Description="L3 相电流"},
        };
        /// <summary>
        /// 日冻结电量曲线的默认捕获对象集合
        /// </summary>
        public static ObservableCollection<CaptureObjectDefinition> Day = Energy;
    }
}