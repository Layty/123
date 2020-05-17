using System.Linq;

namespace 三相智慧能源网关调试软件.Model.IIC
{
    public class IicHarmonicData
    {
        public string Harmonic1 { get; set; }
        public string Harmonic2 { get; set; }
        public string Harmonic3 { get; set; }
        public string Harmonic4 { get; set; }
        public string Harmonic5 { get; set; }
        public string Harmonic6 { get; set; }
        public string Harmonic7 { get; set; }
        public string Harmonic8 { get; set; }
        public string Harmonic9 { get; set; }
        public string Harmonic10 { get; set; }
        public string Harmonic11 { get; set; }
        public string Harmonic12 { get; set; }
        public string Harmonic13 { get; set; }
        public string Harmonic14 { get; set; }
        public string Harmonic15 { get; set; }
        public string Harmonic16 { get; set; }
        public string Harmonic17 { get; set; }
        public string Harmonic18 { get; set; }
        public string Harmonic19 { get; set; }
        public string Harmonic20 { get; set; }
        public string Harmonic21 { get; set; }

        public bool ParseData(byte[] inputBytes)
        {
            if (inputBytes.Length == 0)
            {
                return false;
            }

            Harmonic1 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(0), ("%", 2), 2);
            Harmonic2 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(2), ("%", 2), 2);
            Harmonic3 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(4), ("%", 2), 2);
            Harmonic4 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(6), ("%", 2), 2);
            Harmonic5 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(8), ("%", 2), 2);
            Harmonic6 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(10), ("%", 2), 2);
            Harmonic7 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(12), ("%", 2), 2);
            Harmonic8 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(14), ("%", 2), 2);
            Harmonic9 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(16), ("%", 2), 2);
            Harmonic10 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(18), ("%", 2), 2);
            Harmonic11 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(20), ("%", 2), 2);
            Harmonic12 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(22), ("%", 2), 2);
            Harmonic13 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(24), ("%", 2), 2);
            Harmonic14 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(26), ("%", 2), 2);
            Harmonic15 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(28), ("%", 2), 2);
            Harmonic16 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(30), ("%", 2), 2);
            Harmonic17 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(32), ("%", 2), 2);
            Harmonic18 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(34), ("%", 2), 2);
            Harmonic19 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(36), ("%", 2), 2);
            Harmonic20 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(38), ("%", 2), 2);
            Harmonic21 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(40), ("%", 2), 2);
            return true;
        }
    }
}