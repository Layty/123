using System;
using System.Linq;
using 三相智慧能源网关调试软件.Commom;


namespace 三相智慧能源网关调试软件.Model.IIC
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/05/14 11:41:15
        主要用途：
        更改记录：
    */

    public class IicInstantData
    {
        public byte[] StartFrame = new byte[] {0x80, 0x01};
        public string DateTime { get; private set; }
        public string Ua { get; private set; }
        public string Ub { get; private set; }
        public string Uc { get; private set; }
        public string Ia { get; private set; }
        public string Ib { get; private set; }
        public string Ic { get; private set; }
        public string In { get; private set; }

        public string P { get; private set; }
        public string Pa { get; private set; }
        public string Pb { get; private set; }
        public string Pc { get; private set; }

        public string Q { get; private set; }
        public string Qa { get; private set; }
        public string Qb { get; private set; }
        public string Qc { get; private set; }
        public string S { get; private set; }
        public string Sa { get; private set; }
        public string Sb { get; private set; }
        public string Sc { get; private set; }
        public string Pf { get; private set; }
        public string Pfa { get; private set; }
        public string Pfb { get; private set; }
        public string Pfc { get; private set; }
        public string AngleUa { get; private set; }
        public string AngleUb { get; private set; }
        public string AngleUc { get; private set; }
        public string AngleIa { get; private set; }
        public string AngleIb { get; private set; }
        public string AngleIc { get; private set; }

        public string PosActT { get; private set; } //当前正向有功需量
        public string RevActT { get; private set; }
        public string PosReactT { get; private set; }
        public string RevReactT { get; private set; }
        public string RtcBattV { get; private set; }

        public string RunMode { get; private set; }
        public string ErrStatus { get; private set; }
        public string StealStatus { get; private set; }
        public string EmuStatus { get; private set; }
        public string RunStatus { get; private set; }
        public string CurRate { get; private set; }

        public string Freq { get; private set; }

        /// <summary>
        /// 80 10 00 00 01 5C 00 00 00 00 00 00 00 42 00 00 00 B3 00 00 00 66 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 BA 19
        /// </summary>
        /// <param name="inputBytes"></param>
        public bool ParseData(string inputBytes)
        {
            try
            {
                if (inputBytes.Length == 0)
                {
                    return false;
                }

                var bytes = inputBytes.StringToByte();
                if (bytes[0] == 0x80 && bytes[1] == 0x01)
                {
                    var dateTimeBytes = bytes.Skip(2).Take(6).ToArray();
                    foreach (var variable in dateTimeBytes)
                    {
                        DateTime += Convert.ToString(variable, 10).PadLeft(2, '0');
                    }

                    //DateTime = bytes.Skip(2).Take(6).ToArray().ByteToString();
                    Ua = IicCommonDataConvertor.ValueConvertor(bytes.Skip(8), ("V", 3));
                    Ub = IicCommonDataConvertor.ValueConvertor(bytes.Skip(12), ("V", 3));
                    Uc = IicCommonDataConvertor.ValueConvertor(bytes.Skip(16), ("V", 3));

                    Ia = IicCommonDataConvertor.ValueConvertor(bytes.Skip(20), ("A", 3));
                    Ib = IicCommonDataConvertor.ValueConvertor(bytes.Skip(24), ("A", 3));
                    Ic = IicCommonDataConvertor.ValueConvertor(bytes.Skip(28), ("A", 3));
                    In = IicCommonDataConvertor.ValueConvertor(bytes.Skip(32), ("A", 3));

                    P = IicCommonDataConvertor.ValueConvertor(bytes.Skip(36), ("W", 3), isUnsigned: false);
                    Pa = IicCommonDataConvertor.ValueConvertor(bytes.Skip(40), ("W", 3), isUnsigned: false);
                    Pb = IicCommonDataConvertor.ValueConvertor(bytes.Skip(44), ("W", 3), isUnsigned: false);
                    Pc = IicCommonDataConvertor.ValueConvertor(bytes.Skip(48), ("W", 3), isUnsigned: false);

                    Q = IicCommonDataConvertor.ValueConvertor(bytes.Skip(52), ("var", 3), isUnsigned: false);
                    Qa = IicCommonDataConvertor.ValueConvertor(bytes.Skip(56), ("var", 3), isUnsigned: false);
                    Qb = IicCommonDataConvertor.ValueConvertor(bytes.Skip(60), ("var", 3), isUnsigned: false);
                    Qc = IicCommonDataConvertor.ValueConvertor(bytes.Skip(64), ("var", 3), isUnsigned: false);

                    S = IicCommonDataConvertor.ValueConvertor(bytes.Skip(68), ("VA", 3));
                    Sa = IicCommonDataConvertor.ValueConvertor(bytes.Skip(72), ("VA", 3));
                    Sb = IicCommonDataConvertor.ValueConvertor(bytes.Skip(76), ("VA", 3));
                    Sc = IicCommonDataConvertor.ValueConvertor(bytes.Skip(80), ("VA", 3));

                    Pf = IicCommonDataConvertor.ValueConvertor(bytes.Skip(84), ("", 4), isUnsigned: false);
                    Pfa = IicCommonDataConvertor.ValueConvertor(bytes.Skip(88), ("", 4), isUnsigned: false);
                    Pfb = IicCommonDataConvertor.ValueConvertor(bytes.Skip(92), ("", 4), isUnsigned: false);
                    Pfc = IicCommonDataConvertor.ValueConvertor(bytes.Skip(96), ("", 4), isUnsigned: false);

                    AngleUa = IicCommonDataConvertor.ValueConvertor(bytes.Skip(100), ("", 3));
                    AngleUb = IicCommonDataConvertor.ValueConvertor(bytes.Skip(104), ("", 3));
                    AngleUc = IicCommonDataConvertor.ValueConvertor(bytes.Skip(108), ("", 3));
                    AngleIa = IicCommonDataConvertor.ValueConvertor(bytes.Skip(112), ("", 3));
                    AngleIb = IicCommonDataConvertor.ValueConvertor(bytes.Skip(116), ("", 3));
                    AngleIc = IicCommonDataConvertor.ValueConvertor(bytes.Skip(120), ("", 3));

                    PosActT = IicCommonDataConvertor.ValueConvertor(bytes.Skip(124), ("W", 3));
                    RevActT = IicCommonDataConvertor.ValueConvertor(bytes.Skip(128), ("W", 3));
                    PosReactT = IicCommonDataConvertor.ValueConvertor(bytes.Skip(132), ("var", 3));
                    RevReactT = IicCommonDataConvertor.ValueConvertor(bytes.Skip(136), ("var", 3));

                    RtcBattV = IicCommonDataConvertor.ValueConvertor(bytes.Skip(140), ("V", 2));

                    Freq = IicCommonDataConvertor.ValueConvertor(bytes.Skip(144), ("Hz", 2), 2);
                    CurRate = IicCommonDataConvertor.ValueConvertor(bytes.Skip(146), ("", 0), 1);
                    RunMode = IicCommonDataConvertor.ValueConvertor(bytes.Skip(147), ("", 0), 1);
                    ErrStatus = IicCommonDataConvertor.ValueConvertor(bytes.Skip(148), ("", 0), 2);
                    StealStatus = IicCommonDataConvertor.ValueConvertor(bytes.Skip(150), ("", 0));
                    EmuStatus = IicCommonDataConvertor.ValueConvertor(bytes.Skip(154), ("", 0));
                    RunStatus = IicCommonDataConvertor.ValueConvertor(bytes.Skip(158), ("", 0), 2);
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }

            return false;
        }
    }


    public class IicHarmonicData
    {
        public byte[] StartFrame = new byte[] {0x80, 0x30};
        //public string Harmonic0 { get; set; }
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

            Harmonic1  = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(0), ("%", 2), 2);
            Harmonic2 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(2), ("%", 2), 2);
            Harmonic3 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(4), ("%", 2), 2);
            Harmonic4 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(6), ("%", 2), 2);
            Harmonic5 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(8), ("%", 2), 2);
            Harmonic6 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(10), ("%", 2), 2);
            Harmonic7 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(12), ("%", 2), 2);
            Harmonic8 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(14), ("%", 2), 2);
            Harmonic9 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(16), ("%", 2), 2);
            Harmonic10= IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(18), ("%", 2), 2);
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