using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;

namespace 三相智慧能源网关调试软件.Model
{

    public class PagerMaster
    {

    }
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






  
        private int _count;
        public int Count
        {
            get { return _count; }
            set { _count = value; OnPropertyChanged("Count"); CurrentPageChanged(); }
        }
        private int _countPerPage = 10;
        public int CountPerPage
        {
            get { return _countPerPage; }
            set { _countPerPage = value; this.OnPropertyChanged("CountPerPage"); CurrentPageChanged(); }
        }
        private int _current = 1;
        public int Current
        {
            get { return _current; }
            set { _current = value; this.OnPropertyChanged("Current"); CurrentPageChanged(); }
        }
        public ObservableCollection<DlmsStructure> PaginationCollection { get; set; } = new ObservableCollection<DlmsStructure>();
        private void CurrentPageChanged()
        {
            PaginationCollection.Clear();

            foreach (var i in Buffer.Skip((Current - 1) * CountPerPage).Take(CountPerPage))
            {
                PaginationCollection.Add(i);
            }
        }

        public int TotalPage { get; set; }
    }
}
      

       

    