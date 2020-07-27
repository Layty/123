using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using 三相智慧能源网关调试软件.Annotations;

namespace 三相智慧能源网关调试软件.Model.ENetConfig
{
    public class ENetSetRequest : INotifyPropertyChanged
    {

        public long Timestamp
        {
            get => _timestamp;
            set { _timestamp = value; OnPropertyChanged(); }
        }
        private long _timestamp;


        public ENetEventType EventType
        {
            get => _eventType;
            set { _eventType = value; OnPropertyChanged(); }
        }
        private ENetEventType _eventType;


        public int MyProperty
        {
            get => _MyProperty;
            set { _MyProperty = value; OnPropertyChanged(); }
        }
        private int _MyProperty;

        public ENetSetRequest(ENetEventType eventType)
        {
            Timestamp = 0;
            this.EventType = eventType;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ENetMessageBuilder:INotifyPropertyChanged
    {
        public long Timestamp
        {
            get => _timestamp;
            set { _timestamp = value; OnPropertyChanged(); }
        }
        private long _timestamp;

        
        public ENetEventType EventType
        {
            get => _eventType;
            set { _eventType = value; OnPropertyChanged(); }
        }
        private ENetEventType _eventType;

        public ENetMessageBuilder(ENetEventType eventType)
        {
            Timestamp = 0;
            this.EventType = eventType;
        }

        public string GetRequest()
        {
            var str = JsonConvert.SerializeObject(this);
            var data = Encoding.UTF8.GetBytes(str);
            var base64Str = Convert.ToBase64String(data);
            return base64Str;
        }
        public string SetRequest()
        {
            var str = JsonConvert.SerializeObject(this);
            var data = Encoding.UTF8.GetBytes(str);
            var base64Str = Convert.ToBase64String(data);
            return base64Str;
        }

        public event PropertyChangedEventHandler PropertyChanged;

      
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}