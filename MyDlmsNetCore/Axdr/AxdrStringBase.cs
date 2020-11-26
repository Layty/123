using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MyDlmsNetCore.Axdr
{
    public abstract class AxdrStringBase : IToPduStringInHex, IPduStringInHexConstructor, INotifyPropertyChanged
    {
        [XmlIgnore] public virtual int Length => CalculateLength();

        private int CalculateLength()
        {
            int num = 0;
            if (Value != null)
            {
                num += Value.Length / 2;
            }

            return num;
        }

        private string _value;

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

        public abstract string ToPduStringInHex();


        public abstract bool PduStringInHexConstructor(ref string pduStringInHex);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}