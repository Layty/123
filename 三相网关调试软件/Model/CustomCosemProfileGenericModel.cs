using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
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
            CurrentPage = 1;

            
            _firstPageCommand = new RelayCommand(FirstPageAction);

            _previousPageCommand = new RelayCommand(PreviousPageAction);

            _nextPageCommand = new RelayCommand(NextPageAction);

            _lastPageCommand = new RelayCommand(LastPageAction);
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


        private ICommand _firstPageCommand;

        public ICommand FirstPageCommand
        {
            get
            {
                return _firstPageCommand;
            }

            set
            {
                _firstPageCommand = value;
            }
        }

        private ICommand _previousPageCommand;

        public ICommand PreviousPageCommand
        {
            get
            {
                return _previousPageCommand;
            }

            set
            {
                _previousPageCommand = value;
            }
        }

        private ICommand _nextPageCommand;

        public ICommand NextPageCommand
        {
            get
            {
                return _nextPageCommand;
            }

            set
            {
                _nextPageCommand = value;
            }
        }

        private ICommand _lastPageCommand;

        public ICommand LastPageCommand
        {
            get
            {
                return _lastPageCommand;
            }

            set
            {
                _lastPageCommand = value;
            }
        }

        private int _pageSize;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    if (Buffer.Count % PageSize != 0)
                    {
                        TotalPage = (Buffer.Count / _pageSize) + 1;

                    }
                    else
                    {
                        TotalPage = Buffer.Count / _pageSize;
                    }
                    FirstPageAction();
                   
                    OnPropertyChanged("PageSize");
                }
            }
        }

        private int _currentPage;

        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                
                if (_currentPage != value)
                {
                    if (value == _currentPage)
                        return;
                    _currentPage = value;
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        private int _totalPage;

        public int TotalPage
        {
            get
            {
                return _totalPage;
            }

            set
            {
                if (_totalPage != value)
                {
                    _totalPage = value;
                    FirstPageAction();
                    OnPropertyChanged();
                }
            }
        }

        private List<DlmsStructure> _fakeSoruce = new List<DlmsStructure>();

        public List<DlmsStructure> FakeSource
        {
            get
            {
                return _fakeSoruce;
            }
            set
            {
                if (_fakeSoruce != value)
                {
                    _fakeSoruce = value;
                    OnPropertyChanged("FakeSource");
                }
            }
        }

        public void FirstPageAction()
        {
            CurrentPage = 1;
            var result = Buffer.Take(_pageSize).ToList();
            FakeSource = result;
        }

        private void PreviousPageAction()
        {
            if (CurrentPage == 1)
            {
                return;
            }

            List<DlmsStructure> result = new List<DlmsStructure>();

            if (CurrentPage == 2)
            {
                result = Buffer.Take(_pageSize).ToList();
            }
            else
            {
                result = Buffer.Skip((CurrentPage - 2) * _pageSize).Take(_pageSize).ToList();
            }

            //FakeSource.Clear();

            //FakeSource.AddRange(result);
            FakeSource = result;

            CurrentPage--;
        }

        private void NextPageAction()
        {
            if (CurrentPage == _totalPage)
            {
                return;
            }

            List<DlmsStructure> result = new List<DlmsStructure>();

            result = Buffer.Skip(CurrentPage * _pageSize).Take(_pageSize).ToList();

            //FakeSource.Clear();

            //FakeSource.AddRange(result);
            FakeSource = result;

            CurrentPage++;
        }

        private void LastPageAction()
        {
            CurrentPage = TotalPage;

            int skipCount = (_totalPage - 1) * _pageSize;
            int takeCount = Buffer.Count - skipCount;

            var result = Buffer.Skip(skipCount).Take(takeCount).ToList();

            //FakeSource.Clear();

            //FakeSource.AddRange(result);
            FakeSource = result;
        }
    }
}