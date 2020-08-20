using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public enum OctetStringDisplayFormat
    {
        Original,
        Ascii,
        DateTime,
        Date,
        Time,
        Obis
    }

    public enum UInt32ValueDisplayFormat
    {
        Original,
        IpAddress,
        IntValue,
    }

    public enum SelfDisplayEnum
    {
        None,
        OctetString,
        UInt32
    }

    public class ValueDisplay:INotifyPropertyChanged
    {
        [XmlIgnore] public SelfDisplayEnum DisplayEnum { get; set; }
        public string ValueString { get=>_valueString;
            set { _valueString = value;OnPropertyChanged(); }
        }
        private string _valueString;
        [XmlAttribute] public OctetStringDisplayFormat OctetStringDisplayFormat { get=>_octetStringDisplayFormat;
            set { _octetStringDisplayFormat = value;OnPropertyChanged(); }
        }
        private OctetStringDisplayFormat _octetStringDisplayFormat;
        [XmlAttribute] public UInt32ValueDisplayFormat UInt32ValueDisplayFormat { get=>_uInt32ValueDisplayFormat;
            set { _uInt32ValueDisplayFormat = value;OnPropertyChanged(); }
        }
        private UInt32ValueDisplayFormat _uInt32ValueDisplayFormat;
        public ValueDisplay(OctetStringDisplayFormat octetString, UInt32ValueDisplayFormat uInt32Value)
        {
            OctetStringDisplayFormat = octetString;
            UInt32ValueDisplayFormat = uInt32Value;
        }

        public ValueDisplay()
        {
            OctetStringDisplayFormat = OctetStringDisplayFormat.Original;
            UInt32ValueDisplayFormat = UInt32ValueDisplayFormat.Original;
        }

     

        public void UpdateDisplayFormat(byte[] ValueBytes, DataType dataType, OctetStringDisplayFormat octetString, UInt32ValueDisplayFormat uInt32Value)
        {
            OctetStringDisplayFormat = octetString;
            UInt32ValueDisplayFormat = uInt32Value;
            if (dataType == DataType.OctetString)
            {
                DisplayEnum = SelfDisplayEnum.OctetString;
                if (DisplayEnum == SelfDisplayEnum.OctetString)
                {
                    ValueString = NormalDataParse.HowToDisplayOctetString(ValueBytes, OctetStringDisplayFormat);
                }
            }
            else if (dataType == DataType.UInt32)
            {
                DisplayEnum = SelfDisplayEnum.UInt32;
                if (DisplayEnum == SelfDisplayEnum.UInt32)
                {
                    ValueString = NormalDataParse.HowToDisplayIntValue(ValueBytes, UInt32ValueDisplayFormat);
                }
            }
            

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DLMSDataItem : IToPduBytes, INotifyPropertyChanged
    {
        public void UpdateDisplayFormat(OctetStringDisplayFormat octetString,UInt32ValueDisplayFormat uInt32Value)
        {
            if (DataType!=DataType.OctetString)
            {
                if (DataType != DataType.UInt32)
                {
                    return;
                }
            
            }
           
            ValueDisplay.UpdateDisplayFormat(ValueBytes, DataType, octetString,uInt32Value);
         
        }

        public ValueDisplay ValueDisplay
        {
            get => _valueDisplay;
            set { _valueDisplay = value;OnPropertyChanged(); }
        }

        private ValueDisplay _valueDisplay;

        [XmlAttribute]
        public DataType DataType
        {
            get => _dataType;
            set
            {
                _dataType = value;
                OnPropertyChanged();
            }
        }

        private DataType _dataType;

        [XmlIgnore]
        public byte[] ValueBytes
        {
            get => _valueBytes;
            set
            {
                _valueBytes = value;
                OriginalHexValue = value.ByteToString();
                OnPropertyChanged();
            }
        }

        private byte[] _valueBytes;

        [XmlAttribute]
        public string OriginalHexValue
        {
            get => _originalHexValue;
            set
            {
                _originalHexValue = value;
                OnPropertyChanged();
            }
        }

        private string _originalHexValue;


        public void UpdateValueBytes()
        {
            setValueByte(DataType, ValueDisplay.ValueString);
        }


        public DLMSDataItem()
        {
            DataType = DataType.NullData;
        }

        public DLMSDataItem(DataType dataType, byte[] valueBytes)
        {
            DataType = dataType;
            ValueBytes = valueBytes;
        }

        public DLMSDataItem(DataType dataType, string hexString)
        {
            DataType = dataType;
            ValueDisplay = new ValueDisplay();
            ValueDisplay.ValueString = hexString;
            ParseDLMSDataItem(DataType, hexString);
          
        }

        private void setValueByte(DataType dataType, string valueString)
        {
            switch (dataType)
            {
                case DataType.UInt8:
                    ValueBytes = new[] {byte.Parse(valueString)};
                    break;
                case DataType.UInt16:
                    ValueBytes = BitConverter.GetBytes(ushort.Parse(valueString)).Reverse().ToArray();
                    break;
                case DataType.UInt32:
                    ValueDisplay.DisplayEnum = SelfDisplayEnum.UInt32;
                    switch (ValueDisplay.UInt32ValueDisplayFormat)
                    {
                        case UInt32ValueDisplayFormat.Original:
                            ValueBytes = valueString.StringToByte();
                            break;
                        case UInt32ValueDisplayFormat.IpAddress:
                        {
                            var s = valueString.Split('.');
                            List<byte> list = new List<byte>();
                            foreach (var VARIABLE in s)
                            {
                                list.Add(byte.Parse(VARIABLE));
                            }

                            ValueBytes = list.ToArray();
                            break;
                        }
                        case UInt32ValueDisplayFormat.IntValue:
                            ValueBytes = BitConverter.GetBytes(uint.Parse(valueString)).Reverse().ToArray();
                            break;
                    }

                    break;
                case DataType.OctetString:
                    ValueDisplay.DisplayEnum = SelfDisplayEnum.OctetString;
                    byte[] dataBytes;
                    switch (ValueDisplay.OctetStringDisplayFormat)
                    {
                        case OctetStringDisplayFormat.Ascii:
                            dataBytes = Encoding.Default.GetBytes(valueString);
                            break;

                        default:
                            dataBytes = valueString.StringToByte();
                            break;
                    }

                    if (dataBytes.Length != 0)
                    {
                        byte len = (byte) dataBytes.Length;
                        List<byte> list = new List<byte>();
                        list.Add(len);
                        list.AddRange(dataBytes);
                        ValueBytes = list.ToArray();
                    }

                    break;
                case DataType.BitString:
                    ValueBytes = valueString.StringToByte().Skip(1).ToArray();
                    var count = valueString.StringToByte()[0];
                    var value = valueString.StringToByte().Skip(1).ToArray();
                    var bitstring = new DLMSBitString(value,0,count);
                    break;
                case DataType.String:
                    var data = Encoding.Default.GetBytes(valueString);
                    var dataLength = (byte) data.Length;
                    List<byte> ls = new List<byte>();
                    ls.Add(dataLength);
                    ls.AddRange(data);
                    ValueBytes = ls.ToArray();
                    break;
                case DataType.Boolean:
                    ValueBytes = new[] { byte.Parse(valueString) };
                    break;
                case DataType.Enum:
                    ValueBytes = new[] { byte.Parse(valueString) };
                    break;
            }
        }

        public void ParseDLMSDataItem(DataType dataType, string hexString)
        {
            switch (dataType)
            {
                case DataType.BitString:
                    ValueBytes = hexString.StringToByte().Skip(1).ToArray();
                    var count = hexString.StringToByte()[0];
                    var value = hexString.StringToByte().Skip(1).ToArray();
                    ValueDisplay.ValueString=new DLMSBitString(value,0,count).Value;
                    break;
                case DataType.UInt8:
                    ValueBytes = new[] {byte.Parse(hexString)};
                    break;

                case DataType.UInt16:
                    ValueBytes = BitConverter.GetBytes(ushort.Parse(hexString)).Reverse().ToArray();
                    break;
                case DataType.UInt32:
                    ValueBytes = BitConverter.GetBytes(uint.Parse(hexString)).Reverse().ToArray();
                    break;
                case DataType.OctetString:
                    ValueBytes = hexString.StringToByte();
                    break;

                case DataType.String:
                    ValueBytes = hexString.StringToByte();
                    ValueDisplay.ValueString = Encoding.Default.GetString(ValueBytes);
                    break;
                case DataType.Boolean:
                    ValueBytes = new[] { byte.Parse(hexString) };
                    break;
                case DataType.Enum:
                    ValueBytes = new[] { byte.Parse(hexString) };
                    break;
            }
        }

        public string GXBitString(byte value, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ToBitString(stringBuilder, value, 8);
            if (count != 8)
            {
                stringBuilder.Remove(count, 8 - count);
            }

            return stringBuilder.ToString();
        }

        public void ToBitString(StringBuilder stringBuilder, byte value, int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (count > 8)
            {
                count = 8;
            }

            for (int num = 7; num != 8 - count - 1; num--)
            {
                if ((value & (1 << num)) != 0)
                {
                    stringBuilder.Append('1');
                }
                else
                {
                    stringBuilder.Append('0');
                }
            }
        }

        public byte[] ToPduBytes()
        {
            var list = new List<byte> {(byte) DataType};
            list.AddRange(ValueBytes);
            return list.ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string EncodeVarLength(int qty)
        {
            if (qty <= 127)
            {
                return qty.ToString("X2");
            }

            if (qty <= 255)
            {
                return "81" + qty.ToString("X2");
            }

            return "82" + qty.ToString("X4");
        }
    }
}