using System;
using System.Linq;
using System.Text;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.Model.IIC
{
    public class IicParameter
    {
        public string FactSoftVer { get; set; } //自定义软件版本号（ASCII）
        public string FactHardVer { get; set; } //自定义硬件版本号（ASCII）
        public string PulseConst { get; set; } //脉冲常数

        public string MeterNo { get; set; } //表号


        public string NetType { get; set; }
        public string LocalAddr { get; set; }
        public string Mask { get; set; }
        public string GateWay { get; set; }

        public string RemoteValid1 { get; set; } //启用标志(=1:启用 其它值:未启用)
        public string RemoteAddr1 { get; set; } //IP地址
        public string RemotePort1 { get; set; } //IP端口号

        public string RemoteValid2 { get; set; }
        public string RemoteAddr2 { get; set; }
        public string RemotePort2 { get; set; }
        public string RemoteValid3 { get; set; }
        public string RemoteAddr3 { get; set; }
        public string RemotePort3 { get; set; }
        public string RemoteValid4 { get; set; }
        public string RemoteAddr4 { get; set; }
        public string RemotePort4 { get; set; }

        public string GPRSDialupValid { get; set; } //启用标志(=1:启用 其它值:未启用)
        public string Apn { get; set; }
        public string ApnUser { get; set; }
        public string ApnPwd { get; set; }
        public string Serial { get; set; } //拨号码

        //        public IicLocalIp LocalIp { get; set; } //网关IP参数=
        //
        //        public RemoteIp RemoteIp { get; set; }
        //        public GPRSDialup GprsDialup { get; set; }

        public IicParameter()
        {
//            LocalIp = new IicLocalIp();
//            RemoteIp = new RemoteIp();
//            GprsDialup = new GPRSDialup();
        }

        public bool ParseData(byte[] inputBytes)
        {
            if (inputBytes.Length == 0)
            {
                return false;
            }

            try
            {
                //   FactSoftVer = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(0), ("", 0), 32);
                FactSoftVer = Encoding.Default.GetString(inputBytes.Skip(0).Take(32).ToArray());
                //   FactHardVer = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(32), ("", 0), 32);
                FactHardVer = Encoding.Default.GetString(inputBytes.Skip(32).Take(32).ToArray());
                PulseConst = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(64), ("imp", 0), 4);
//                MeterNo = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(68), ("imp", 0), 16);
                MeterNo = inputBytes.Skip(68).Take(16).ToArray().ByteToString();

                NetType = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(84), ("", 0), 1);
                //LocalAddr = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(85), ("", 0), 4);
                LocalAddr = inputBytes.Skip(85).Take(4).ToArray().ByteToString(" ");
                //Mask = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(89), ("", 0), 4);
                Mask = inputBytes.Skip(89).Take(4).ToArray().ByteToString(" ");
                //    GateWay = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(93), ("", 0), 4);
                GateWay = inputBytes.Skip(93).Take(4).ToArray().ByteToString(" ");

                RemoteValid1 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(97), ("", 0), 1);
                //  RemoteAddr1 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(98), ("", 0), 4);
                RemoteAddr1 = inputBytes.Skip(98).Take(4).ToArray().ByteToString(" ");
                RemotePort1 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(102), ("", 0), 2);

                RemoteValid2 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(104), ("", 0), 1);
                //  RemoteAddr2 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(105), ("", 0), 4);
                RemoteAddr2 = inputBytes.Skip(105).Take(4).ToArray().ByteToString(" ");
                RemotePort2 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(109), ("", 0), 2);

                RemoteValid3 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(111), ("", 0), 1);
                // RemoteAddr3 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(112), ("", 0), 4);
                RemoteAddr3 = inputBytes.Skip(112).Take(4).ToArray().ByteToString(" ");
                RemotePort3 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(116), ("", 0), 2);

                RemoteValid4 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(118), ("", 0), 1);
                // RemoteAddr4 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(119), ("", 0), 4);
                RemoteAddr4 = inputBytes.Skip(119).Take(4).ToArray().ByteToString(" ");
                RemotePort4 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(123), ("", 0), 2);


                GPRSDialupValid = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(125), ("", 0), 1);
                // Apn = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(126), ("", 0), 20);
                Apn = inputBytes.Skip(126).Take(20).ToArray().ByteToString(" ");
                // ApnUser = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(146), ("", 0), 20);
                ApnUser = inputBytes.Skip(146).Take(20).ToArray().ByteToString(" ");
                //                ApnPwd = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(166), ("", 0), 20);
                ApnPwd = inputBytes.Skip(166).Take(20).ToArray().ByteToString(" ");
                // Serial = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(186), ("", 0), 20);
                Serial = inputBytes.Skip(186).Take(20).ToArray().ByteToString(" ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return true;
        }
    }

    public class GPRSDialup
    {
        public string Valid { get; set; } //启用标志(=1:启用 其它值:未启用)
        public string Apn { get; set; }
        public string ApnUser { get; set; }
        public string ApnPwd { get; set; }
        public string Serial { get; set; } //拨号码
    }

    public struct RemoteIp
    {
        public string Valid { get; set; } //启用标志(=1:启用 其它值:未启用)
        public string Addrs { get; set; } //IP地址
        public string Port { get; set; } //IP端口号
    }

    public class IicLocalIp
    {
        public string NetType { get; set; }
        public string Addr { get; set; }
        public string Mask { get; set; }
        public string GateWay { get; set; }
    }

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
            try
            {
                if (inputBytes.Length == 0)
                {
                    return false;
                }
                else
                {
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}