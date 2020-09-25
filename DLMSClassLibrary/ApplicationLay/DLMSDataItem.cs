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
using 三相智慧能源网关调试软件.DLMS.Common;

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

    public class ValueDisplay : INotifyPropertyChanged
    {
        [XmlIgnore] public SelfDisplayEnum DisplayEnum { get; set; }

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

        [XmlAttribute]
        public OctetStringDisplayFormat OctetStringDisplayFormat
        {
            get => _octetStringDisplayFormat;
            set
            {
                _octetStringDisplayFormat = value;
                OnPropertyChanged();
            }
        }

        private OctetStringDisplayFormat _octetStringDisplayFormat;

        [XmlAttribute]
        public UInt32ValueDisplayFormat UInt32ValueDisplayFormat
        {
            get => _uInt32ValueDisplayFormat;
            set
            {
                _uInt32ValueDisplayFormat = value;
                OnPropertyChanged();
            }
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


        public void UpdateDisplayFormat(byte[] ValueBytes, DataType dataType, OctetStringDisplayFormat octetString,
            UInt32ValueDisplayFormat uInt32Value)
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

    [XmlInclude(typeof(DlmsStructure))]
    public class DLMSDataItem : IToPduStringInHex, IToPduBytes, INotifyPropertyChanged
    {
        public void UpdateDisplayFormat(OctetStringDisplayFormat octetString, UInt32ValueDisplayFormat uInt32Value)
        {
            if (DataType != DataType.OctetString)
            {
                if (DataType != DataType.UInt32)
                {
                    return;
                }
            }

            ValueDisplay.UpdateDisplayFormat(ValueBytes, DataType, octetString, uInt32Value);
        }

        public ValueDisplay ValueDisplay
        {
            get => _valueDisplay;
            set
            {
                _valueDisplay = value;
                OnPropertyChanged();
            }
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
            ValueDisplay = new ValueDisplay();
        }

        public DLMSDataItem(DataType dataType, byte[] valueBytes)
        {
            DataType = dataType;
            ValueDisplay = new ValueDisplay();
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
                    var bitstring = new DLMSBitString(value, 0, count);
                    break;
                case DataType.VisibleString:
                    var data = Encoding.Default.GetBytes(valueString);
                    var dataLength = (byte) data.Length;
                    List<byte> ls = new List<byte>();
                    ls.Add(dataLength);
                    ls.AddRange(data);
                    ValueBytes = ls.ToArray();
                    break;
                case DataType.Boolean:
                    ValueBytes = new[] {byte.Parse(valueString)};
                    break;
                case DataType.Enum:
                    ValueBytes = new[] {byte.Parse(valueString)};
                    break;
                case DataType.Structure:

                    break;
            }
        }

   
        public byte[] ParseDLMSDataItem(DataType dataType, string hexString)
        {
            switch (dataType)
            {
                case DataType.BitString:
                    ValueBytes = hexString.StringToByte().Skip(1).ToArray();
                    var count = hexString.StringToByte()[0];
                    var value = hexString.StringToByte().Skip(1).ToArray();
                    ValueDisplay.ValueString = new DLMSBitString(value, 0, count).Value;
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

                case DataType.VisibleString:
                    ValueBytes = hexString.StringToByte();
                    ValueDisplay.ValueString = Encoding.Default.GetString(ValueBytes);
                    break;
                case DataType.Boolean:
                    ValueBytes = new[] {byte.Parse(hexString)};
                    break;
                case DataType.Enum:
                    ValueBytes = new[] {byte.Parse(hexString)};
                    break;
                case DataType.Structure:
                    ValueBytes = hexString.StringToByte().ToArray();
                    //var StructureCount = hexString.StringToByte()[0];

                    //List<byte> StructureList = new List<byte>();
                    //for (int i = 0; i < StructureCount; i++)
                    //{
                    //    StructureList.AddRange(ParseDLMSDataItem((DataType,)));
                    //}


                    break;
            }

            return ValueBytes;
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


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            try
            {
                switch (pduStringInHex.Substring(0, 2))
                {
                    case "03":
                        DataType = DataType.Boolean;
                        ValueDisplay.ValueString = pduStringInHex.Substring(2, 2);
                        ValueBytes = ValueDisplay.ValueString.StringToByte();
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "04":
                        DataType = DataType.BitString;
                        pduStringInHex = pduStringInHex.Substring(2);
                        ValueDisplay.ValueString = BitStringConstructor(ref pduStringInHex);
                        ValueBytes = ValueDisplay.ValueString.StringToByte();
                        break;
                    case "05":
                        DataType = DataType.Int32;
                        ValueBytes = pduStringInHex.Substring(2, 8).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToInt32(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "06":
                        DataType = DataType.UInt32;
                        ValueBytes = pduStringInHex.Substring(2, 8).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToUInt32(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "09":
                        DataType = DataType.OctetString;
                        pduStringInHex = pduStringInHex.Substring(2);
                        ValueDisplay.ValueString = OctetStringConstructor(ref pduStringInHex);
                        ValueBytes = ValueDisplay.ValueString.StringToByte();
                        break;
                    case "0A":
                        DataType = DataType.VisibleString;
                        pduStringInHex = pduStringInHex.Substring(2);
                        ValueDisplay.ValueString = VisibleStringConstructor(ref pduStringInHex);
                        ValueBytes = Encoding.Default.GetBytes(ValueDisplay.ValueString);
                        break;
                    case "0F":
                        DataType = DataType.Int8;
                        ValueBytes = pduStringInHex.Substring(2, 2).StringToByte();
                        ValueDisplay.ValueString = (ValueBytes[0].ToString());
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "10":
                        DataType = DataType.Int16;
                        ValueBytes = pduStringInHex.Substring(2, 4).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToInt16(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(6);
                        break;
                    case "11":
                        DataType = DataType.UInt8;
                        ValueBytes = pduStringInHex.Substring(2, 2).StringToByte();
                        ValueDisplay.ValueString = (ValueBytes[0].ToString());
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "12":
                        DataType = DataType.UInt16;
                        ValueBytes = pduStringInHex.Substring(2, 4).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToInt16(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(6);
                        break;
                    case "13":
                    //{
                    //    DataType = DataType.CompactArray;
                    //    pduStringInHex = pduStringInHex.Substring(2);
                    //    DlmsCompactArray dlmsCompactArray = new DlmsCompactArray();
                    //    if (!dlmsCompactArray.PduStringInHexConstructor(ref pduStringInHex))
                    //    {
                    //        return false;
                    //    }
                    //    value = dlmsCompactArray;
                    //    break;
                    //}
                    case "14":
                        DataType = DataType.Int64;
                        ValueBytes = pduStringInHex.Substring(2, 16).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToInt64(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(18);
                        break;
                    case "15":
                        DataType = DataType.UInt64;
                        ValueBytes = pduStringInHex.Substring(2, 16).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToUInt64(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(18);
                        break;
                    case "16":
                        DataType = DataType.Enum;
                        ValueBytes = pduStringInHex.Substring(2, 2).StringToByte();
                        ValueDisplay.ValueString = (ValueBytes[0].ToString());
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "17":
                        DataType = DataType.Float32;
                        ValueBytes = pduStringInHex.Substring(2, 8).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToSingle(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "18":
                        DataType = DataType.Float64;
                        ValueBytes = pduStringInHex.Substring(2, 16).StringToByte();
                        ValueDisplay.ValueString = BitConverter.ToDouble(ValueBytes.Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(18);
                        break;
                    case "19":
                        DataType = DataType.DateTime;
                        ValueBytes = pduStringInHex.Substring(2, 24).StringToByte();
                        pduStringInHex = pduStringInHex.Substring(26);
                        break;
                    case "1A":
                        DataType = DataType.Date;
                        ValueBytes = pduStringInHex.Substring(2, 10).StringToByte();
                        pduStringInHex = pduStringInHex.Substring(12);
                        break;
                    case "1B":
                        DataType = DataType.Time;
                        ValueBytes = pduStringInHex.Substring(2, 8).StringToByte();
                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "02":
                        DataType = DataType.Structure;
                        pduStringInHex = pduStringInHex.Substring(2);
                        if (!((new DlmsStructure())).PduStringInHexConstructor(ref pduStringInHex))
                        {
                            return false;
                        }

                        ValueBytes = pduStringInHex.StringToByte();
                        break;
                    case "01":
                        DataType = DataType.Array;
                        pduStringInHex = pduStringInHex.Substring(2);
                        if (!((new DLMSArray())).PduStringInHexConstructor(ref pduStringInHex))
                        {
                            return false;
                        }

                        break;
                    case "00":
                        DataType = DataType.NullData;
                        ValueBytes = null;
                        pduStringInHex = pduStringInHex.Substring(2);
                        break;
                    default:
                        throw new Exception("Unrecognized Tag");
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        private string OctetStringConstructor(ref string s)
        {
            int num = MyConvert.DecodeVarLength(ref s);
            string result = s.Substring(0, num * 2);
            s = s.Substring(num * 2);
            return result;
        }

        private string VisibleStringConstructor(ref string s)
        {
            int num = MyConvert.DecodeVarLength(ref s);
            string s2 = s.Substring(0, num * 2);
            s = s.Substring(num * 2);
            return MyConvert.OctetStringToString(s2);
        }

        

        public string ToPduStringInHex()
        {
            switch (DataType)
            {
                case DataType.Boolean:
                    return "03" + ValueBytes.ByteToString("");
                case DataType.BitString:
                    return "04" + GetBitStringValue(ValueBytes.ByteToString(""));
                case DataType.Int32:
                    return "05" + ValueBytes.ByteToString();
                case DataType.UInt32:
                    return "06" + ValueBytes.ByteToString();
                case DataType.OctetString:
                    return "09" + ValueBytes.Length+(ValueBytes.ByteToString(""));
                case DataType.VisibleString:
                    return "0A" + Encoding.Default.GetString(ValueBytes).Length.ToString("X2")+ValueBytes.ByteToString("");
//                case "INTEGER":
//                    return "0F" + value.ToString();
//                case "LONG":
//                    return "10" + value.ToString();
//                case "UNSIGNED":
//                    return "11" + value.ToString();
//                case "LONGUNSIGNED":
//                    return "12" + value.ToString();
//                case "LONG64":
//                    return "14" + value.ToString();
//                case "LONG64UNSIGNED":
//                    return "15" + value.ToString();
//                case "ENUM":
//                    return "16" + value.ToString();
//                case "FLOAT32":
//                    return "17" + value.ToString();
//                case "FLOAT64":
//                    return "18" + value.ToString();
//                case "DATETIME":
//                    return "19" + value.ToString();
//                case "DATE":
//                    return "1A" + value.ToString();
//                case "TIME":
//                    return "1B" + value.ToString();
//                case "STRUCTURE":
//                {
//                    DlmsStructure dlmsStructure = (DlmsStructure)value;
//                    return dlmsStructure.ToPduStringInHex();
//                }
//                case "ARRAY":
//                {
//                    DlmsArray dlmsArray = (DlmsArray)value;
//                    return dlmsArray.ToPduStringInHex();
//                }
//                case "COMPACTARRAY":
//                {
//                    DlmsCompactArray dlmsCompactArray = (DlmsCompactArray)value;
//                    return dlmsCompactArray.ToPduStringInHex();
//                }
                case DataType.NullData:
                    return "00";
                default:
                    throw new Exception("Unrecognized Type");
            }
        }

        private string GetBitStringValue(string s)
        {
            return EncodeVarLength(s.Length) + BitStringToHexByteString(s);
        }
        private string BitStringConstructor(ref string s)
        {
            int num = MyConvert.DecodeVarLength(ref s);
            int num2 = (num + 7) / 8;
            string text = s.Substring(0, num2 * 2);
            s = s.Substring(num2 * 2);
            byte b = 0;
            byte b2 = 0;
            int num3 = 0;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                if (b == 0)
                {
                    b = 128;
                    b2 = Convert.ToByte(text.Substring(num3, 2), 16);
                    num3 += 2;
                }

                if ((b & b2) != 0)
                {
                    stringBuilder.Append('1');
                }
                else
                {
                    stringBuilder.Append('0');
                }

                b = (byte)(b >> 1);
            }

            return stringBuilder.ToString();
        }
        private string BitStringToHexByteString(string bitString)
        {
            int length = bitString.Length;
            byte b = 0;
            byte b2 = 128;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                switch (bitString[i])
                {
                    case '1':
                        b = (byte)(b | b2);
                        break;
                    default:
                        throw new Exception("Illegal character in BitString");
                    case '0':
                        break;
                }
                b2 = (byte)(b2 >> 1);
                if (b2 == 0)
                {
                    stringBuilder.Append(b.ToString("X2"));
                    b = 0;
                    b2 = 128;
                }
            }
            if (b2 != 128)
            {
                stringBuilder.Append(b.ToString("X2"));
            }
            return stringBuilder.ToString();
        }

    }
}