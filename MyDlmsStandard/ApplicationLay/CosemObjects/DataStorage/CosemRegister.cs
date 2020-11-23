using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    /// <summary>
    ///是一个结构体，count=2,Scalar ,Unit
    /// </summary>
    public class ScalarUnit : IPduStringInHexConstructor, INotifyPropertyChanged
    {
        public DataType DataType { get; set; } = DataType.Structure;

        public DlmsDataItem Scalar
        {
            get => _scalar;
            set
            {
                _scalar = value;
                OnPropertyChanged();
            }
        }

        private DlmsDataItem _scalar;

        public DlmsDataItem Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        private DlmsDataItem _unit;


        //        public UnitEnum UnitEnumDisplay
        //        {
        //            get => _unitEnum;
        //            set
        //            {
        //                _unitEnum = value;
        //                OnPropertyChanged();
        //            }
        //        }
        //
        //        private UnitEnum _unitEnum;


        public string UnitDisplay
        {
            get => _unitDisplay;
            set
            {
                _unitDisplay = value;
                OnPropertyChanged();
            }
        }

        private string _unitDisplay;

        public ScalarUnit()
        {
            Scalar = new DlmsDataItem();
            Unit = new DlmsDataItem();
        }


        public static Dictionary<byte, string> ScalarUnitDefinitionDictionary => GetScalarUnitDefinition();

        public static Dictionary<byte, string> GetScalarUnitDefinition()
        {
            Dictionary<byte, string> dictionary = new Dictionary<byte, string>();
            dictionary.Add(0, "None");
            dictionary.Add(1, "a");
            dictionary.Add(2, "mo");
            dictionary.Add(3, "wk");
            dictionary.Add(4, "d");
            dictionary.Add(5, "h");
            dictionary.Add(6, "min.");
            dictionary.Add(7, "s");
            dictionary.Add(8, "°");
            dictionary.Add(9, "°C");
            dictionary.Add(10, "currency");
            dictionary.Add(11, "m");
            dictionary.Add(12, "m/s");
            dictionary.Add(13, "m³");
            dictionary.Add(14, "m³");
            dictionary.Add(15, "m³/h");
            dictionary.Add(16, "m³/h");
            dictionary.Add(17, "m³/d");
            dictionary.Add(18, "m³/d");
            dictionary.Add(19, "l");
            dictionary.Add(20, "kg");
            dictionary.Add(21, "N");
            dictionary.Add(22, "Nm");
            dictionary.Add(23, "Pa");
            dictionary.Add(24, "bar");
            dictionary.Add(25, "J");
            dictionary.Add(26, "J/h");
            dictionary.Add(27, "W");
            dictionary.Add(28, "VA");
            dictionary.Add(29, "var");
            dictionary.Add(30, "Wh");
            dictionary.Add(31, "VAh");
            dictionary.Add(32, "varh");
            dictionary.Add(33, "A");
            dictionary.Add(34, "C");
            dictionary.Add(35, "V");
            dictionary.Add(36, "V/m");
            dictionary.Add(37, "F");
            dictionary.Add(38, "Ʊ");
            dictionary.Add(39, "Ʊm²/m");
            dictionary.Add(40, "Wb");
            dictionary.Add(41, "T");
            dictionary.Add(42, "A/m");
            dictionary.Add(43, "H");
            dictionary.Add(44, "Hz");
            dictionary.Add(45, "1/(Wh)");
            dictionary.Add(46, "1/(varh)");
            dictionary.Add(47, "1/(VAh)");
            dictionary.Add(48, "V²h");
            dictionary.Add(49, "A²h");
            dictionary.Add(50, "kg/s");
            dictionary.Add(51, "S, mho");
            dictionary.Add(52, "K");
            dictionary.Add(53, "1/(V²h)");
            dictionary.Add(54, "1/(A²h)");
            dictionary.Add(55, "1/m³");
            dictionary.Add(56, "%");
            dictionary.Add(57, "Ah");
            dictionary.Add(60, "Wh/m³");
            dictionary.Add(61, "J/m³");
            dictionary.Add(62, "Mol %");
            dictionary.Add(63, "g/m³");
            dictionary.Add(64, "Pa s");
            dictionary.Add(65, "J.kg");
            dictionary.Add(70, "dBm");
            dictionary.Add(71, "dbµV");
            dictionary.Add(72, "dB");
            dictionary.Add(254, "");
            dictionary.Add(byte.MaxValue, "");
            return dictionary;
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            var t = new DlmsStructure();
            if (!t.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            if (t.Items.Length != 2)
            {
                return false;
            }

            if (t.Items[0].DataType != DataType.Int8)
            {
                return false;
            }

            Scalar = t.Items[0];

            if (t.Items[1].DataType != DataType.Enum)
            {
                return false;
            }

            Unit = t.Items[1];

            var by = Convert.ToByte(Unit.Value.ToString(), 16);
            UnitDisplay = ScalarUnitDefinitionDictionary[by];
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CosemRegister : CosemData
    {
        public ScalarUnit ScalarUnit
        {
            get => _scalarUnit;
            set
            {
                _scalarUnit = value;
                OnPropertyChanged();
            }
        }

        private ScalarUnit _scalarUnit;

        public CosemRegister(string logicName) : base(logicName, ObjectType.Register)
        {
            LogicalName = logicName;
        }

        public CosemAttributeDescriptor GetScalar_UnitAttributeDescriptor() => GetCosemAttributeDescriptor(3);

        public CosemMethodDescriptor GetResetMethodDescriptor() => GetCosemMethodDescriptor(1);

        public override string[] GetNames()
        {
            return new[] {LogicalName, "Value", "Scalar_Unit"};
        }

        public override int GetAttributeCount() => 3;


        public override int GetMethodCount() => 1;


        public override DataType GetDataType(int index)
        {
            DataType dataType = new DataType();
            switch (index)
            {
                case 1:
                    dataType = DataType.OctetString;
                    break;
                case 2:
                    dataType = Value.DataType;
                    break;
                case 3:
                    dataType = DataType.Structure;
                    break;
            }

            return dataType;
        }
    }
}