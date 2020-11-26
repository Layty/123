using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MyDlmsNetCore.Axdr
{//T L V 

    public abstract class AxdrIntegerBase : IToPduStringInHex, IPduStringInHexConstructor, INotifyPropertyChanged
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

        public  string ToPduStringInHex()
        {
            return Value;
        }


        public  bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < Length)
            {
                return false;
            }

            Value = pduStringInHex.Substring(0, Length*2);
            pduStringInHex = pduStringInHex.Substring(Length * 2);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}