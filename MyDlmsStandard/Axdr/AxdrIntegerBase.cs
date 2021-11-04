using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MyDlmsStandard.Axdr
{ //T L V 

    public abstract class AxdrIntegerBase<T> : IToPduStringInHex, IPduStringInHexConstructor,
        INotifyPropertyChanged, IGetEntityValue<T> where T : struct
    {

        [XmlIgnore] public virtual int Length { get; set; }

        [XmlAttribute]
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private string _value;


        public string ToPduStringInHex()
        {
            return Value;
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < Length * 2)
            {
                return false;
            }

            Value = pduStringInHex.Substring(0, Length * 2);
            pduStringInHex = pduStringInHex.Substring(Length * 2);
            return true;
        }

        public virtual T GetEntityValue()
        {
            return default;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}