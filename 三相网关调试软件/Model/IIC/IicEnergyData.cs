using System;
using System.Linq;

namespace 三相智慧能源网关调试软件.Model.IIC
{
    public  class IicEnergyData
    {
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
        public string Quad1ReactT0 { get; set; }
        public string Quad1ReactT1 { get; set; }
        public string Quad1ReactT2 { get; set; }
        public string Quad1ReactT3 { get; set; }
        public string Quad1ReactT4 { get; set; }
        public string Quad2ReactT0 { get; set; }
        public string Quad2ReactT1 { get; set; }
        public string Quad2ReactT2 { get; set; }
        public string Quad2ReactT3 { get; set; }
        public string Quad2ReactT4 { get; set; }
        public string Quad3ReactT0 { get; set; }
        public string Quad3ReactT1 { get; set; }
        public string Quad3ReactT2 { get; set; }
        public string Quad3ReactT3 { get; set; }
        public string Quad3ReactT4 { get; set; }
        public string Quad4ReactT0 { get; set; }
        public string Quad4ReactT1 { get; set; }
        public string Quad4ReactT2 { get; set; }
        public string Quad4ReactT3 { get; set; }
        public string Quad4ReactT4 { get; set; }

        public bool ParseData(byte[] inputBytes)
        {
            try
            {
                if (inputBytes.Length == 0)
                {
                    return false;
                }

                {
                    var data = inputBytes.Skip(2).ToArray();
                    PosActT0 = IicCommonDataConvertor.ValueConvertor(data.Skip(0), ("kWh", 3));
                    PosActT1 = IicCommonDataConvertor.ValueConvertor(data.Skip(4), ("kWh", 3));
                    PosActT2 = IicCommonDataConvertor.ValueConvertor(data.Skip(8), ("kWh", 3));
                    PosActT3 = IicCommonDataConvertor.ValueConvertor(data.Skip(12), ("kWh", 3));
                    PosActT4 = IicCommonDataConvertor.ValueConvertor(data.Skip(16), ("kWh", 3));
                    RevActT0 = IicCommonDataConvertor.ValueConvertor(data.Skip(20), ("kWh", 3));
                    RevActT1 = IicCommonDataConvertor.ValueConvertor(data.Skip(24), ("kWh", 3));
                    RevActT2 = IicCommonDataConvertor.ValueConvertor(data.Skip(28), ("kWh", 3));
                    RevActT3 = IicCommonDataConvertor.ValueConvertor(data.Skip(32), ("kWh", 3));
                    RevActT4 = IicCommonDataConvertor.ValueConvertor(data.Skip(36), ("kWh", 3));

                    Quad1ReactT0 = IicCommonDataConvertor.ValueConvertor(data.Skip(40), ("kvarh", 3));
                    Quad1ReactT1 = IicCommonDataConvertor.ValueConvertor(data.Skip(44), ("kvarh", 3));
                    Quad1ReactT2 = IicCommonDataConvertor.ValueConvertor(data.Skip(48), ("kvarh", 3));
                    Quad1ReactT3 = IicCommonDataConvertor.ValueConvertor(data.Skip(52), ("kvarh", 3));
                    Quad1ReactT4 = IicCommonDataConvertor.ValueConvertor(data.Skip(56), ("kvarh", 3));

                    Quad2ReactT0 = IicCommonDataConvertor.ValueConvertor(data.Skip(60), ("kvarh", 3));
                    Quad2ReactT1 = IicCommonDataConvertor.ValueConvertor(data.Skip(64), ("kvarh", 3));
                    Quad2ReactT2 = IicCommonDataConvertor.ValueConvertor(data.Skip(68), ("kvarh", 3));
                    Quad2ReactT3 = IicCommonDataConvertor.ValueConvertor(data.Skip(72), ("kvarh", 3));
                    Quad2ReactT4 = IicCommonDataConvertor.ValueConvertor(data.Skip(76), ("kvarh", 3));

                    Quad3ReactT0 = IicCommonDataConvertor.ValueConvertor(data.Skip(80), ("kvarh", 3));
                    Quad3ReactT1 = IicCommonDataConvertor.ValueConvertor(data.Skip(84), ("kvarh", 3));
                    Quad3ReactT2 = IicCommonDataConvertor.ValueConvertor(data.Skip(88), ("kvarh", 3));
                    Quad3ReactT3 = IicCommonDataConvertor.ValueConvertor(data.Skip(92), ("kvarh", 3));
                    Quad3ReactT4 = IicCommonDataConvertor.ValueConvertor(data.Skip(96), ("kvarh", 3));

                    Quad4ReactT0 = IicCommonDataConvertor.ValueConvertor(data.Skip(100), ("kvarh", 3));
                    Quad4ReactT1 = IicCommonDataConvertor.ValueConvertor(data.Skip(104), ("kvarh", 3));
                    Quad4ReactT2 = IicCommonDataConvertor.ValueConvertor(data.Skip(108), ("kvarh", 3));
                    Quad4ReactT3 = IicCommonDataConvertor.ValueConvertor(data.Skip(112), ("kvarh", 3));
                    Quad4ReactT4 = IicCommonDataConvertor.ValueConvertor(data.Skip(116), ("kvarh", 3));
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return false;
        }

      
    }

}