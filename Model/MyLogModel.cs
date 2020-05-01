using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.Model
{
    public class MyLogModel : ObservableObject
    {
        private string _commandLog;

        public string CommandLog
        {
            get { return _commandLog; }
            set
            {
                _commandLog = value; ;
                RaisePropertyChanged();
            }
        }
    }
}