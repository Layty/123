using System;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemProfileGenericModel : CosemProfileGeneric
    {
        public string ProfileGenericName { get; set; }
        public CustomCosemProfileGenericModel(string logicalName) : base(logicalName)
        {
            FromDateTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));
            ToDateTime = DateTime.Now;
        }
        public DateTime FromDateTime
        {
            get => _fromDateTime;
            set { _fromDateTime = value; OnPropertyChanged(); }
        }
        private DateTime _fromDateTime;

        public DateTime ToDateTime
        {
            get => _toDateTime;
            set { _toDateTime = value; OnPropertyChanged(); }
        }
        private DateTime _toDateTime;
    }
}