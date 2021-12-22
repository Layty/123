using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using System;

namespace JobMaster.Models
{
    /// <summary>
    /// 自定义的曲线类模型，继承自CosemProfileGeneric
    /// </summary>
    public class CustomCosemProfileGenericModel : CosemProfileGeneric
    {
        public string ProfileGenericName { get; set; }

        public CustomCosemProfileGenericModel(string logicalName) : base(logicalName)
        {
            FromDateTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));
            ToDateTime = DateTime.Now;
        }

        /// <summary>
        /// 通过Range方式时，根据Clock的方式检索时提供FromDateTime快速创建指令
        /// </summary>
        public DateTime FromDateTime
        {
            get => _fromDateTime;
            set
            {
                _fromDateTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime _fromDateTime;
        /// <summary>
        /// 通过Range方式时，根据Clock的方式检索时提供ToDateTime快速创建指令
        /// </summary>
        public DateTime ToDateTime
        {
            get => _toDateTime;
            set
            {
                _toDateTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime _toDateTime;



    }


}