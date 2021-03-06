using System;
using System.Linq;

namespace 三相智慧能源网关调试软件.Model.IIC
{
    public class IicDemandData
    {
        public class DemandStruct
        {
            public string Time { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Time + " " + Value;
            }
        }

        public string PosActT0 { get; set; }
        public string PosActT1 { get; set; }
        public string PosActT2 { get; set; }
        public string PosActT3 { get; set; }
        public string PosActT4 { get; set; }

        public string RevActT0 { get; set; }
        public string RevActT1 { get; set; }
        public string RevActT2 { get; set; }
        public string RevActT3 { get; set; }
        public string RevActT4 { get; set; }


        public string PosReactT0 { get; set; }
        public string PosReactT1 { get; set; }
        public string PosReactT2 { get; set; }
        public string PosReactT3 { get; set; }
        public string PosReactT4 { get; set; }

        public string RevReactT0 { get; set; }
        public string RevReactT1 { get; set; }
        public string RevReactT2 { get; set; }
        public string RevReactT3 { get; set; }
        public string RevReactT4 { get; set; }

        private string GetValue(byte[] inputBytes, (string Unit, int Scalar) unitScalar)
        {
            var demand = new DemandStruct
            {
                Value = IicCommonDataConvertor.ValueConvertor(inputBytes, unitScalar)
            };
            var timeBytes = BitConverter.ToUInt32(inputBytes.Skip(4).Reverse().ToArray(), 0);
            demand.Time = new DateTime(2000, 1, 1).AddSeconds(timeBytes).ToString("yy-MM-dd ddd HH:mm:ss");
            return demand.ToString();
        }

        public bool ParseData(byte[] inputBytes)
        {
            if (inputBytes.Length == 0)
            {
                return false;
            }

            if (inputBytes[0] == 0x80)
            {
                var data = inputBytes.Skip(2).ToArray();
                PosActT0 = GetValue(data.Skip(0).Take(8).ToArray(), ("kW", 3));
                PosActT1 = GetValue(data.Skip(8).Take(8).ToArray(), ("kW", 3));
                PosActT2 = GetValue(data.Skip(16).Take(8).ToArray(), ("kW", 3));
                PosActT3 = GetValue(data.Skip(24).Take(8).ToArray(), ("kW", 3));
                PosActT4 = GetValue(data.Skip(32).Take(8).ToArray(), ("kW", 3));
                RevActT0 = GetValue(data.Skip(40).Take(8).ToArray(), ("kW", 3));
                RevActT1 = GetValue(data.Skip(48).Take(8).ToArray(), ("kW", 3));
                RevActT2 = GetValue(data.Skip(56).Take(8).ToArray(), ("kW", 3));
                RevActT3 = GetValue(data.Skip(64).Take(8).ToArray(), ("kW", 3));
                RevActT4 = GetValue(data.Skip(72).Take(8).ToArray(), ("kW", 3));

                PosReactT0 = GetValue(data.Skip(80).Take(8).ToArray(), ("kvarh", 3));
                PosReactT1 = GetValue(data.Skip(88).Take(8).ToArray(), ("kvarh", 3));
                PosReactT2 = GetValue(data.Skip(96).Take(8).ToArray(), ("kvarh", 3));
                PosReactT3 = GetValue(data.Skip(104).Take(8).ToArray(), ("kvarh", 3));
                PosReactT4 = GetValue(data.Skip(112).Take(8).ToArray(), ("kvarh", 3));

                RevReactT0 = GetValue(data.Skip(120).Take(8).ToArray(), ("kvarh", 3));
                RevReactT1 = GetValue(data.Skip(128).Take(8).ToArray(), ("kvarh", 3));
                RevReactT2 = GetValue(data.Skip(136).Take(8).ToArray(), ("kvarh", 3));
                RevReactT3 = GetValue(data.Skip(144).Take(8).ToArray(), ("kvarh", 3));
                RevReactT4 = GetValue(data.Skip(152).Take(8).ToArray(), ("kvarh", 3));
                return true;
            }


            return false;
        }
    }
}