using System;
using System.Linq;
using System.Text;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.Model.IIC
{
    public class IicParameter
    {
        /// <summary>
        /// 自定义软件版本号（ASCII）32字节
        /// </summary>
        public string FactSoftVer { get; set; }

        /// <summary>
        /// 自定义硬件版本号（ASCII）32字节
        /// </summary>
        public string FactHardVer { get; set; }

        /// <summary>
        /// 脉冲常数  4字节
        /// </summary>
        public string PulseConst { get; set; }

        /// <summary>
        /// 表号 16字节
        /// </summary>
        public string MeterNo { get; set; }

        /// <summary>
        ///网关 IP 参数 网络类型 = 1:DHCP 2:手动 其它值:无效
        /// </summary>
        public string NetType { get; set; }

        /// <summary>
        /// 网关 IP 参数 本机IP
        /// </summary>
        public string LocalAddr { get; set; }

        /// <summary>
        /// 网关 IP 参数 子网掩码
        /// </summary>
        public string Mask { get; set; }

        /// <summary>
        /// 网关 IP 参数 默认网关
        /// </summary>
        public string GateWay { get; set; }

        /// <summary>
        /// 第1路主站通信参数 启用标志(=1:启用 其它值:未启用)
        /// </summary>
        public string RemoteValid1 { get; set; }

        /// <summary>
        /// 第1路主站通信参数 IP地址
        /// </summary>
        public string RemoteAddr1 { get; set; }

        /// <summary>
        /// 第1路主站通信参数 IP端口号
        /// </summary>
        public string RemotePort1 { get; set; }


        /// <summary>
        /// 第2路主站通信参数 启用标志(=1:启用 其它值:未启用)
        /// </summary>
        public string RemoteValid2 { get; set; }

        /// <summary>
        /// 第2路主站通信参数 IP地址
        /// </summary>
        public string RemoteAddr2 { get; set; }

        /// <summary>
        /// 第2路主站通信参数 IP端口号
        /// </summary>
        public string RemotePort2 { get; set; }

        /// <summary>
        /// 第3路主站通信参数 启用标志(=1:启用 其它值:未启用)
        /// </summary>
        public string RemoteValid3 { get; set; }

        /// <summary>
        /// 第3路主站通信参数 IP地址
        /// </summary>
        public string RemoteAddr3 { get; set; }

        /// <summary>
        /// 第3路主站通信参数 IP端口号
        /// </summary>
        public string RemotePort3 { get; set; }

        /// <summary>
        /// 第4路主站通信参数 启用标志(=1:启用 其它值:未启用)
        /// </summary>
        public string RemoteValid4 { get; set; }

        /// <summary>
        /// 第4路主站通信参数 IP地址
        /// </summary>
        public string RemoteAddr4 { get; set; }

        /// <summary>
        /// 第4路主站通信参数 IP端口号
        /// </summary>
        public string RemotePort4 { get; set; }

        /// <summary>
        /// 全网通模块拨号参数 启用标志(=1:启用 其它值:未启用)
        /// </summary>
        public string GPRSDialupValid { get; set; }

        /// <summary>
        /// 全网通模块拨号参数 apn
        /// </summary>
        public string Apn { get; set; }

        /// <summary>
        /// 全网通模块拨号参数 ApnUser
        /// </summary>
        public string ApnUser { get; set; }

        /// <summary>
        /// 全网通模块拨号参数 ApnPwd
        /// </summary>
        public string ApnPwd { get; set; }

        /// <summary>
        /// 全网通模块拨号参数 拨号码
        /// </summary>
        public string Serial { get; set; }


        public bool ParseData(byte[] inputBytes)
        {
            if (inputBytes.Length == 0)
            {
                return false;
            }

            inputBytes = inputBytes.Skip(2).ToArray();
            try
            {
                FactSoftVer = Encoding.Default.GetString(inputBytes.Skip(0).Take(32).ToArray());
                FactHardVer = Encoding.Default.GetString(inputBytes.Skip(32).Take(32).ToArray());
                ;
                PulseConst = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(64), ("imp/kWh", 0), 4);
                MeterNo = inputBytes.Skip(68).Take(16).ToArray().ByteToString();

                NetType = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(84), ("", 0), 1);

                LocalAddr = inputBytes.Skip(85).Take(4).ToArray().ByteToString(" ");

                Mask = inputBytes.Skip(89).Take(4).ToArray().ByteToString(" ");

                GateWay = inputBytes.Skip(93).Take(4).ToArray().ByteToString(" ");

                RemoteValid1 = IicCommonDataConvertor.ValueConvertor(inputBytes.Skip(97), ("", 0), 1);

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
}