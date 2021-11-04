using CommonServiceLocator;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.Model.ENetConfig;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class ENetMessageBuilderViewModel : ObservableObject
    {
        public ENetMessageBuilder ENetMessageMaker
        {
            get => _ENetMessageMaker;
            set
            {
                _ENetMessageMaker = value;
                OnPropertyChanged();
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
                OnPropertyChanged(); ;
            }
        }

        private RelayCommand _requestCommand;


        public string ResultStr
        {
            get => _ResultStr;
            set
            {
                _ResultStr = value;
                OnPropertyChanged(); ;
            }
        }

        private string _ResultStr;


        public RelayCommand SetCommand
        {
            get => _SetCommand;
            set
            {
                _SetCommand = value;
                OnPropertyChanged(); ;
            }
        }

        private RelayCommand _SetCommand;


        public ObservableCollection<object> ItemsCollection
        {
            get => _ItemsCollection;
            set
            {
                _ItemsCollection = value;
                OnPropertyChanged(); ;
            }
        }

        private ObservableCollection<object> _ItemsCollection;


        public ENetMessageBuilderViewModel()
        {
            ENetMessageMaker = new ENetMessageBuilder(ENetEventType.SortVersion);
            ItemsCollection = new ObservableCollection<object>();
            ResultStr.StringToByte();
            StrongReferenceMessenger.Default.Register<byte[], string>(this, "ENetReceiveDataEvent", (sender, args) =>
             {
                 var base64 = Convert.FromBase64String(Encoding.Default.GetString(args));
                 ResultStr = ConvertJsonString(Encoding.Default.GetString(base64));
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