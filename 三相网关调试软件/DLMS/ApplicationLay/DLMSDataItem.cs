using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
            set
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
                _valueString = value;
                OnPropertyChanged();
            }
        }

        private string _valueString;



        public DisplayFormatToShow DisplayFormat
        {
            get => _displayFormat;
            set
            {
                _displayFormat = value;
                if (DataType==DataType.OctetString)
                {
                    ValueString = NormalDataParse.HowToDisplayOctetString(ValueBytes, value);
                }
               
                OnPropertyChanged();
            }
        }

        private DisplayFormatToShow _displayFormat = DisplayFormatToShow.Original;


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
            switch (DataType)
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
                    ValueBytes = valueString.StringToByte();break;
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