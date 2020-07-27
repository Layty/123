using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using 三相智慧能源网关调试软件.Model.ENetConfig;

namespace 三相智慧能源网关调试软件.ViewModel
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/06/28 14:20:13
        主要用途：
        更改记录：
    */
    public class ENetMessageBuilderViewModel : ViewModelBase
    {
        public ENetMessageBuilder ENetMessageMaker
        {
            get => _ENetMessageMaker;
            set
            {
                _ENetMessageMaker = value;
                RaisePropertyChanged();
            }
        }

        private ENetMessageBuilder _ENetMessageMaker;


        public Array ENetEventTypeCollection => Enum.GetValues(typeof(ENetEventType));

        public RelayCommand RequestCommand
        {
            get => _requestCommand;
            set
            {
                _requestCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _requestCommand;


        public string ResultStr
        {
            get => _ResultStr;
            set
            {
                _ResultStr = value;
                RaisePropertyChanged();
            }
        }

        private string _ResultStr;


        public RelayCommand SetCommand
        {
            get => _SetCommand;
            set
            {
                _SetCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _SetCommand;


        public ObservableCollection<object> ItemsCollection
        {
            get => _ItemsCollection;
            set
            {
                _ItemsCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<object> _ItemsCollection;


        public ENetMessageBuilderViewModel()
        {
            ENetMessageMaker = new ENetMessageBuilder(ENetEventType.SortVersion);
            ItemsCollection = new ObservableCollection<object>();
            Messenger.Default.Register<byte[]>(this, "ENetReceiveDataEvent", (t) =>
            {
                var base64 = Convert.FromBase64String(Encoding.Default.GetString(t));
                ResultStr = ConvertJsonString(Encoding.Default.GetString(base64));

                //var data = JsonConvert.DeserializeObject<object>(ResultStr);
                //DispatcherHelper.CheckBeginInvokeOnUI(() => { ItemsCollection.Add(data); });
            });
            RequestCommand = new RelayCommand(() =>
            {
                var ENetClient = ServiceLocator.Current.GetInstance<ENetClientHelper>();
                var e = ENetMessageMaker.GetRequest();
                ENetClient.SendDataToServer(e);
            });
            SetCommand = new RelayCommand(() =>
            {
                var ENetClient = ServiceLocator.Current.GetInstance<ENetClientHelper>();
                var e = ENetMessageMaker.GetRequest();
                ENetClient.SendDataToServer(e);
            });
        }

        private string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }
    }
}