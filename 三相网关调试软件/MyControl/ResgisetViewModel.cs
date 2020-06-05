using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;

namespace 三相智慧能源网关调试软件.MyControl
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/06/04 16:06:26
        主要用途：
        更改记录：
    */
    public class RegisterViewModel : ViewModelBase
    {
        public ObservableCollection<DLMSRegister> Registers
        {
            get => _Registers;
            set
            {
                _Registers = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DLMSRegister> _Registers;


        public RelayCommand<DLMSRegister> GetValueCommand
        {
            get => _GetValueCommand;
            set
            {
                _GetValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSRegister> _GetValueCommand;


        private RelayCommand _GetRequestCommand;

        public string DataForShow
        {
            get => _dataForShow;
            set
            {
                _dataForShow = value;
                RaisePropertyChanged();
            }
        }

        private string _dataForShow;
        public DLMSClient Client { get; set; }

        public RegisterViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            Registers = new ObservableCollection<DLMSRegister>()
            {
                new DLMSRegister("0.1.24.2.30.255"),
                new DLMSRegister("0.1.24.2.31.255"),
            };

            GetValueCommand = new RelayCommand<DLMSRegister>(
                async t =>
                {
                    t.Value = "";
                    var dataResult = await Client.GetRequest(t.GetValue());
                    t.Value = NormalDataParse.ParsePduData(dataResult);
                    var Scalar_Unit = await Client.GetRequest(t.GetScalar_Unit());
                    t.Value += NormalDataParse.ParsePduData(Scalar_Unit);
                });
        }
    }
}