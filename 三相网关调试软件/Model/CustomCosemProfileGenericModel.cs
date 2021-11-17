using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using System;
using System.Collections.ObjectModel;
using System.Linq;


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


        private int _count;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged();
                CurrentPageChanged();
            }
        }

        private int _countPerPage = 10;

        public int CountPerPage
        {
            get { return _countPerPage; }
            set
            {
                _countPerPage = value;
                OnPropertyChanged();
                CurrentPageChanged();
            }
        }

        private int _current = 1;

        public int Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged();
                CurrentPageChanged();
            }
        }

        public ObservableCollection<DlmsStructure> PaginationCollection { get; set; } =
            new ObservableCollection<DlmsStructure>();

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