using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using System;

namespace JobMaster.Models
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
            set
            {
                _fromDateTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime _fromDateTime;

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