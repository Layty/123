using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public enum DisplayFormatToShow
    {
        Original,
        ASCII,
        DateTime,
        Date,
        Time,
        OBIS
    }

    public class DLMSDataItem : IToPduBytes, INotifyPropertyChanged
    {
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


        public byte[] ValueBytes
        {
            get => _valueBytes;
            private set
            {
                _valueBytes = value;
                OnPropertyChanged();
            }
        }

        private byte[] _valueBytes;

        public string ValueString
        {
            get => _valueString;
            set
            {
                setValueByte(DataType, value);
                _valueString = value;
                OnPropertyChanged();
            }
        }

        private string _valueString;


        public DLMSDataItem()
        {
        }

        public DLMSDataItem(DataType dataType, byte[] valueBytes)
        {
            DataType = dataType;
            ValueBytes = valueBytes;
        }

        public DLMSDataItem(DataType dataType, string valueString)
        {
            DataType = dataType;
            ValueString = valueString;
            ParseDLMSDataItem(DataType, valueString);
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
                    ValueBytes = BitConverter.GetBytes(uint.Parse(valueString)).Reverse().ToArray();
                    break;
                case DataType.OctetString:
                    ValueBytes = valueString.StringToByte();
                    break;
                case DataType.BitString:
                    ValueBytes = valueString.StringToByte().Skip(1).ToArray();
                    var count = valueString.StringToByte()[0];
                    var value = valueString.StringToByte().Skip(1).ToArray();

                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (byte value2 in value)
                    {
                        //if (index != 0)
                        //{
                        //    index--;
                        //    continue;
                        //}
                        if (count < 1)
                        {
                            break;
                        }

                        ToBitString(stringBuilder, value2, count);
                        count -= 8;
                    }

                    break;
                case DataType.String:
                    var data = Encoding.Default.GetBytes(valueString);
                    var dataLength = (byte) data.Length;
                    List<byte> ls = new List<byte>();
                    ls.Add(dataLength);
                    ls.AddRange(data);
                    ValueBytes = ls.ToArray();

                    break;
            }
        }

        public void ParseDLMSDataItem(DataType dataType, string valueString)
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
                    ValueBytes = BitConverter.GetBytes(uint.Parse(valueString)).Reverse().ToArray();
                    break;
                case DataType.OctetString:
                    ValueBytes = valueString.StringToByte();
                    break;
                case DataType.BitString:
                    ValueBytes = valueString.StringToByte().Skip(1).ToArray();
                    var count = valueString.StringToByte()[0];
                    var value = valueString.StringToByte().Skip(1).ToArray();

                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (byte value2 in value)
                    {
                        //if (index != 0)
                        //{
                        //    index--;
                        //    continue;
                        //}
                        if (count < 1)
                        {
                            break;
                        }

                        ToBitString(stringBuilder, value2, count);
                        count -= 8;
                    }

                    ValueString = stringBuilder.ToString();
                    break;
                case DataType.String:
                    ValueString = Encoding.Default.GetString(valueString.StringToByte());
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
    }
}