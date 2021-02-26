using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    /// <summary>
    /// 扩展寄存器 class_id=4,版本=0
    /// </summary>
    public class CosemExtendedRegister : CosemRegister
    {
        /// <summary>
        /// 属性4
        /// </summary>
        public DlmsDataItem Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        private DlmsDataItem _status;

        /// <summary>
        /// 提供“Extended register”的具体日期和时间信息,以指示属性value 捕获的 时间。
        /// </summary>
        public AxdrOctetString CaptureTime
        {
            get => _captureTime;
            set
            {
                _captureTime = value;
                OnPropertyChanged();
            }
        }

        private AxdrOctetString _captureTime;
        public virtual CosemAttributeDescriptor GetStatusAttributeDescriptor() => GetCosemAttributeDescriptor(4);
        public virtual CosemAttributeDescriptor GetCaptureTimeAttributeDescriptor() => GetCosemAttributeDescriptor(5);

        public virtual CosemMethodDescriptor GetResetMethodDescriptor(DlmsDataItem data) => GetCosemMethodDescriptor(1);

        public void Reset(DlmsDataItem data)
        {
            Value = data;
        }
        public CosemExtendedRegister(string logicName) : base(logicName)
        {
            LogicalName = logicName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.ExtendedRegister);
        }
    }
}