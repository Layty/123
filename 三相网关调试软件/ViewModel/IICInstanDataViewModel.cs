using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using 三相智慧能源网关调试软件.Model.IIC;

namespace 三相智慧能源网关调试软件.ViewModel
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/05/14 16:06:49
        主要用途：
        更改记录：
    */
    public class IICInstanDataViewModel : ViewModelBase
    {
        public ObservableCollection<IICInstantData> IICInstantDatas
        {
            get => _IICInstantDatas;
            set
            {
                _IICInstantDatas = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IICInstantData> _IICInstantDatas;


        public IICInstantData IICInstantData
        {
            get => _IICInstantData;
            set
            {
                _IICInstantData = value;
                RaisePropertyChanged();
            }
        }

        private IICInstantData _IICInstantData;

        public IICInstanDataViewModel()
        {
            if (IsInDesignMode)
            {
                IICInstantDatas = new ObservableCollection<IICInstantData>() {new IICInstantData() {Ia = "123A"}};
            }

            IICInstantData = new IICInstantData();

            IICInstantDatas = new ObservableCollection<IICInstantData>() {new IICInstantData() {Ia = "123A"}};
            Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent", HandlerdData);
        }

        private void HandlerdData(byte[] obj)
        {
            var stringData = Encoding.Default.GetString(obj);
            var bb = stringData.Split('\n');
            if (bb.Length == 3)
            {
                IICInstantData data = new IICInstantData();
                bool result = data.ParseData(bb[1].Replace('\r',' '));
                if (result)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IICInstantDatas.Add(data);
                    });
                    
                }
            }
        }
    }
}