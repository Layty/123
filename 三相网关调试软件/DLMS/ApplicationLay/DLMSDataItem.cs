using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;
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
        IntValue,
        Original,
        IpAddress,
    }


    [XmlInclude(typeof(DLMSArray))]
    [XmlInclude(typeof(DlmsStructure))]
//    [XmlInclude(typeof(DlmsCompactArray))]
    public class DlmsDataItem : IToPduStringInHex, IPduStringInHexConstructor, INotifyPropertyChanged
    {
        [XmlAttribute]
        public OctetStringDisplayFormat OctetStringDisplayFormat
        {
            get => _octetStringDisplayFormat;
            set
            {
                _octetStringDisplayFormat = value;
                UpdateDisplayFormat();
                OnPropertyChanged();
            }
        }

        private OctetStringDisplayFormat _octetStringDisplayFormat = OctetStringDisplayFormat.Original;

        [XmlAttribute]
        public UInt32ValueDisplayFormat UInt32ValueDisplayFormat
        {
            get => _uInt32ValueDisplayFormat;
            set
            {
                _uInt32ValueDisplayFormat = value;
                UpdateDisplayFormat();
                OnPropertyChanged();
            }
        }

        private UInt32ValueDisplayFormat _uInt32ValueDisplayFormat = UInt32ValueDisplayFormat.IntValue;

        public void UpdateDisplayFormat()
        {
            if (DataType != DataType.OctetString)
            {
                if (DataType != DataType.UInt32)
                {
                    return;
                }
            }


            string formatDisplayOctetString;
            if (DataType == DataType.OctetString)
            {
                try
                {
                    formatDisplayOctetString = NormalDataParse.HowToDisplayOctetString(
                        Value.ToString().StringToByte(),
                        OctetStringDisplayFormat);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                ValueString = formatDisplayOctetString;
            }
            else if (DataType == DataType.UInt32)
            {
                try
                {
                    formatDisplayOctetString = NormalDataParse.HowToDisplayIntValue(
                        Value.ToString().StringToByte(),
                        UInt32ValueDisplayFormat);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                ValueString = formatDisplayOctetString;
            }
        }


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
                OnPropertyChanged();
            }
        }

        private byte[] _valueBytes;

        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                if (value != null)
                {
                    ValueString = value.ToString();
                }

                OnPropertyChanged();
            }
        }

        private object _value;


        public DlmsDataItem()
        {
            DataType = DataType.NullData;
        }

        public DlmsDataItem(DataType dataType)
        {
            DataType = dataType;
        }

        public DlmsDataItem(DataType dataType, object value)
        {
            DataType = dataType;
            Value = value;
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


        public event PropertyChangedEventHandler PropertyChanged;

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
                        Value = pduStringInHex.Substring(2, 2);
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "04":
                        DataType = DataType.BitString;
                        pduStringInHex = pduStringInHex.Substring(2);
                        Value = BitStringConstructor(ref pduStringInHex);

                        break;
                    case "05":
                        DataType = DataType.Int32;
                        Value = pduStringInHex.Substring(2, 8);
                        ValueString = BitConverter
                            .ToInt32(Value.ToString().StringToByte().Reverse().ToArray(), 0).ToString();
                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "06":
                        DataType = DataType.UInt32;
                        Value = pduStringInHex.Substring(2, 8);
//                        ValueString = BitConverter
//                            .ToUInt32(Value.ToString().StringToByte().Reverse().ToArray(), 0).ToString();
                        //特殊修改Uint32
                        UpdateDisplayFormat();

                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "09":
                        DataType = DataType.OctetString;
                        pduStringInHex = pduStringInHex.Substring(2);
                        Value = OctetStringConstructor(ref pduStringInHex);
                        UpdateDisplayFormat();
                        break;
                    case "0A":
                        DataType = DataType.VisibleString;
                        pduStringInHex = pduStringInHex.Substring(2);
                        Value = VisibleStringConstructor(ref pduStringInHex);
                        break;
                    case "0F":
                        DataType = DataType.Int8;
                        Value = pduStringInHex.Substring(2, 2);
                        AxdrIntegerInteger8 i8 = new AxdrIntegerInteger8(Value.ToString());
                        ValueString = i8.GetEntityValue().ToString();
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "10":
                        DataType = DataType.Int16;
                        Value = pduStringInHex.Substring(2, 4);
                        ValueString = Convert.ToInt16(Value.ToString(), 16).ToString();
                        pduStringInHex = pduStringInHex.Substring(6);
                        break;
                    case "11":
                        DataType = DataType.UInt8;
                        Value = pduStringInHex.Substring(2, 2);
                        ValueString = Convert.ToByte(Value.ToString(), 16).ToString();
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "12":
                        DataType = DataType.UInt16;
                        Value = pduStringInHex.Substring(2, 4);
                        ValueString = Convert.ToUInt16(Value.ToString(), 16).ToString();
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
                        Value = pduStringInHex.Substring(2, 16);
                        ValueString = Convert.ToInt64(Value.ToString(), 16).ToString();
                        pduStringInHex = pduStringInHex.Substring(18);
                        break;
                    case "15":
                        DataType = DataType.UInt64;
                        Value = pduStringInHex.Substring(2, 16);
                        ValueString = Convert.ToUInt64(Value.ToString(), 16).ToString();
                        pduStringInHex = pduStringInHex.Substring(18);
                        break;
                    case "16":
                        DataType = DataType.Enum;
                        Value = pduStringInHex.Substring(2, 2);
                        ValueString = Convert.ToByte(Value.ToString(), 16).ToString();
                        pduStringInHex = pduStringInHex.Substring(4);
                        break;
                    case "17":
                        DataType = DataType.Float32;
                        Value = pduStringInHex.Substring(2, 8);
                        ValueString = BitConverter
                            .ToSingle(Value.ToString().StringToByte().Reverse().ToArray(), 0)
                            .ToString();

                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "18":
                        DataType = DataType.Float64;
                        Value = pduStringInHex.Substring(2, 16);
                        ValueString = BitConverter
                            .ToDouble(Value.ToString().StringToByte().Reverse().ToArray(), 0)
                            .ToString();
                        pduStringInHex = pduStringInHex.Substring(18);
                        break;
                    case "19":
                        DataType = DataType.DateTime;
                        Value = pduStringInHex.Substring(2, 24);
                        pduStringInHex = pduStringInHex.Substring(26);
                        break;
                    case "1A":
                        DataType = DataType.Date;
                        Value = pduStringInHex.Substring(2, 10);
                        pduStringInHex = pduStringInHex.Substring(12);
                        break;
                    case "1B":
                        DataType = DataType.Time;
                        Value = pduStringInHex.Substring(2, 8);
                        pduStringInHex = pduStringInHex.Substring(10);
                        break;
                    case "02":
                        DataType = DataType.Structure;
                        pduStringInHex = pduStringInHex.Substring(2);
                        DlmsStructure structure = new DlmsStructure();
                        if (!structure.PduStringInHexConstructor(ref pduStringInHex))
                        {
                            return false;
                        }

                        Value = structure;
                        break;
                    case "01":
                        DataType = DataType.Array;
                        pduStringInHex = pduStringInHex.Substring(2);
                        DLMSArray array = new DLMSArray();
                        if (!array.PduStringInHexConstructor(ref pduStringInHex))
                        {
                            return false;
                        }

                        Value = array;
                        break;
                    case "00":
                        DataType = DataType.NullData;
                        Value = null;
                        pduStringInHex = pduStringInHex.Substring(2);
                        break;
                    default:
                        throw new Exception("Unrecognized Tag");
                }


                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        public string ToPduStringInHex()
        {
            switch (DataType)
            {
                case DataType.Boolean:
                    return "03" + Value;
                case DataType.BitString:
                    return "04" + GetBitStringValue(Value.ToString());
                case DataType.Int32:
                    return "05" + Value;
                case DataType.UInt32:
                    return "06" + Value;
                case DataType.OctetString:
                    return "09" + GetOctetStringValue(Value.ToString());
                case DataType.VisibleString:
                    return "0A" + GetVisibleStringValue(Value.ToString());
                case DataType.Int8:
                    return "0F" + Value;
                case DataType.Int16:
                    return "10" + Value;
                case DataType.UInt8:
                    return "11" + Value;
                case DataType.UInt16:
                    return "12" + Value;
                case DataType.Int64:
                    return "14" + Value;
                case DataType.UInt64:
                    return "15" + Value;
                case DataType.Enum:
                    return "16" + Value;
                case DataType.Float32:
                    return "17" + Value;
                case DataType.Float64:
                    return "18" + Value;
                case DataType.DateTime:
                    return "19" + Value;
                case DataType.Date:
                    return "1A" + Value;
                case DataType.Time:
                    return "1B" + Value;
                case DataType.Structure:
                {
                    DlmsStructure dlmsStructure = (DlmsStructure) Value;
                    return dlmsStructure.ToPduStringInHex();
                }

                case DataType.Array:
                {
                    DLMSArray dlmsArray = (DLMSArray) Value;
                    return dlmsArray.ToPduStringInHex();
                }


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

        public byte[] ToPduBytes()
        {
            return MyConvert.OctetStringToByteArray(ToPduStringInHex());
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

                b = (byte) (b >> 1);
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
                        b = (byte) (b | b2);
                        break;
                    default:
                        throw new Exception("Illegal character in BitString");
                    case '0':
                        break;
                }

                b2 = (byte) (b2 >> 1);
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

        private string GetOctetStringValue(string s)
        {
            int qty = s.Length / 2;
            return EncodeVarLength(qty) + s;
        }

        private string GetVisibleStringValue(string s)
        {
            return EncodeVarLength(s.Length) + MyConvert.StringToOctetString(s);
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

        public void UpdateValue()
        {
            switch (DataType)
            {
                case DataType.UInt8:
                    //                    ValueBytes = new[] {byte.Parse(valueString)};
                    //                    
                    //                    Value = ValueBytes.ByteToString();
                    Value = byte.Parse(ValueString).ToString("X2");
                    AxdrIntegerUnsigned8 uint8 = new AxdrIntegerUnsigned8();

                    break;
                case DataType.UInt16:
                    //                    ValueBytes = BitConverter.GetBytes(ushort.Parse(valueString)).Reverse().ToArray();
                    //                    
                    //                    Value = ValueBytes.ByteToString();
                    Value = ushort.Parse(ValueString).ToString("X4");
                    break;
                case DataType.UInt32:

                    switch (UInt32ValueDisplayFormat)
                    {
                        case UInt32ValueDisplayFormat.Original:
                            ValueBytes = ValueString.StringToByte();
                            Value = ValueBytes.ByteToString();
                            break;
                        case UInt32ValueDisplayFormat.IpAddress:
                            {
                                var s = ValueString.Split('.');
                                List<byte> list = new List<byte>();
                                foreach (var variable in s)
                                {
                                    list.Add(byte.Parse(variable));
                                }

                                ValueBytes = list.ToArray();
                                Value = ValueBytes.ByteToString();
                                break;
                            }
                        case UInt32ValueDisplayFormat.IntValue:
                            ValueBytes = BitConverter.GetBytes(uint.Parse(ValueString)).Reverse().ToArray();
                            Value = ValueBytes.ByteToString();
                            break;
                    }

                    break;
                case DataType.OctetString:
                    byte[] dataBytes;
                    switch (OctetStringDisplayFormat)
                    {
                        case OctetStringDisplayFormat.Ascii:
                            dataBytes = Encoding.Default.GetBytes(ValueString);
                            break;

                        default:
                            dataBytes = ValueString.StringToByte();
                            break;
                    }

                    if (dataBytes.Length != 0)
                    {
                        byte len = (byte)dataBytes.Length;
                        List<byte> list = new List<byte>();
                        //                        list.Add(len);
                        list.AddRange(dataBytes);
                        ValueBytes = list.ToArray();
                        Value = ValueBytes.ByteToString();
                    }

                    break;
                case DataType.BitString:
                    ValueBytes = ValueString.StringToByte().Skip(1).ToArray();
                    Value = ValueBytes.ByteToString();
                    var count = ValueString.StringToByte()[0];
                    var value = ValueString.StringToByte().Skip(1).ToArray();
                    var bitstring = new DLMSBitString(value, 0, count);
                    break;
                case DataType.VisibleString:
                    var data = Encoding.Default.GetBytes(ValueString);
                    var dataLength = (byte)data.Length;
                    List<byte> ls = new List<byte>();
                    ls.Add(dataLength);
                    ls.AddRange(data);
                    ValueBytes = ls.ToArray();
                    Value = ValueBytes.ByteToString();
                    break;
                case DataType.Boolean:
                    ValueBytes = new[] { byte.Parse(ValueString) };
                    Value = ValueBytes.ByteToString();
                    break;
                case DataType.Enum:
                    ValueBytes = new[] { byte.Parse(ValueString) };
                    Value = ValueBytes.ByteToString();
                    break;
                case DataType.Structure:

                    break;
            }
        }
    }
}