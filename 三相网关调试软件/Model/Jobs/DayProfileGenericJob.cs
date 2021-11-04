using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Common;
using System;
using System.Collections.Generic;

namespace 三相智慧能源网关调试软件.Model.Jobs
{
    /// <summary>
    /// 日冻结电量曲线任务
    /// </summary>
    public class DayProfileGenericJob : ProfileGenericJobBase
    {
        public DayProfileGenericJob()
        {
            Period = 60 * 24;
            JobName = "日冻结曲线任务";
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.98.1.1.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                    { AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255" },
                    FromValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Today.Date).GetDateTimeBytes()
                            .ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Now.Date.Add(new TimeSpan(0, 23, 59, 59))).GetDateTimeBytes()
                            .ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                }
            };
        }
    }
}