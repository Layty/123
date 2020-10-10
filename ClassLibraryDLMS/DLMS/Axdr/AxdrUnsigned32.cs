using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ClassLibraryDLMS.DLMS.Axdr
{
    public class AxdrUnsigned32 : IToPduStringInHex, IPduStringInHexConstructor, INotifyPropertyChanged
    {
        [XmlIgnore] public int Length => 4;

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


        public AxdrUnsigned32()
        {
        }

        public AxdrUnsigned32(string s)
        {
            if (s.Length != 8)
            {
                throw new ArgumentException("The length not match type");
            }

            Value = s;
        }

        public string ToPduStringInHex()
        {
            return Value;
        }


        public uint GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToUInt32(Value, 16);
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < 8)
            {
                return false;
            }

            Value = pduStringInHex.Substring(0, 8);
            pduStringInHex = pduStringInHex.Substring(8);
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}