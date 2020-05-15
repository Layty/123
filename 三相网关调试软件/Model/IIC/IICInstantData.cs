using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using 三相智慧能源网关调试软件.Commom;

namespace 三相智慧能源网关调试软件.Model.IIC
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/05/14 11:41:15
        主要用途：
        更改记录：
    */
    public class IICInstantData
    {
        public byte[] StartFrame = new byte[2] {0x80, 0x01};

        public string DateTime { get; set; }
        public string Ua { get; set; }
        public string Ub { get; set; }
        public string Uc { get; set; }
        public string Ia { get; set; }
        public string Ib { get; set; }
        public string Ic { get; set; }
        public string In { get; set; }

        public string P { get; set; }
        public string Pa { get; set; }
        public string Pb { get; set; }
        public string Pc { get; set; }

        public string Q { get; set; }
        public string Qa { get; set; }
        public string Qb { get; set; }
        public string Qc { get; set; }
        public string S { get; set; }
        public string Sa { get; set; }
        public string Sb { get; set; }
        public string Sc { get; set; }
        public string Pf { get; set; }
        public string Pfa { get; set; }
        public string Pfb { get; set; }
        public string Pfc { get; set; }
        public string angle_Ua { get; set; }
        public string angle_Ub { get; set; }
        public string angle_Uc { get; set; }
        public string angle_Ia { get; set; }
        public string angle_Ib { get; set; }
        public string angle_Ic { get; set; }

        public string PosActT { get; set; } //当前正向有功需量
        public string RevActT { get; set; }
        public string PosReactT { get; set; }
        public string RevReactT { get; set; }
        public string RtcBattV { get; set; }

        /// <summary>
        /// 80 10 00 00 01 5C 00 00 00 00 00 00 00 42 00 00 00 B3 00 00 00 66 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 BA 19
        /// </summary>
        /// <param name="inputBytes"></param>
        public bool  ParseData(string inputBytes)
        {
            try
            {
                if (inputBytes.Length==0)
                {
                    return false;
                }
                var bytes = inputBytes.StringToByte();
                if (bytes[0] == 0x80 && bytes[1] == 0x01)
                {
                    var time = bytes.Skip(2).Take(6).ToArray();
                    foreach (var VARIABLE in time)
                    {
                        DateTime += Convert.ToString(VARIABLE,10).PadLeft(2,'0');
                    }
                    //DateTime = bytes.Skip(2).Take(6).ToArray().ByteToString();
                    Ua = ValueConvertor(bytes.Skip(8), ("V", 3));
                    Ub = ValueConvertor(bytes.Skip(12), ("V", 3));
                    Uc = ValueConvertor(bytes.Skip(16), ("V", 3));

                    Ia = ValueConvertor(bytes.Skip(20), ("A", 3));
                    Ib = ValueConvertor(bytes.Skip(24), ("A", 3));
                    Ic = ValueConvertor(bytes.Skip(28), ("A", 3));
                    In = ValueConvertor(bytes.Skip(32), ("A", 3));

                    P = ValueConvertor(bytes.Skip(36), ("W", 3));
                    Pa = ValueConvertor(bytes.Skip(40), ("W", 3));
                    Pb = ValueConvertor(bytes.Skip(44), ("W", 3));
                    Pc = ValueConvertor(bytes.Skip(48), ("W", 3));

                    Q = ValueConvertor(bytes.Skip(52), ("var", 3));
                    Qa = ValueConvertor(bytes.Skip(56), ("var", 3));
                    Qb = ValueConvertor(bytes.Skip(60), ("var", 3));
                    Qc = ValueConvertor(bytes.Skip(64), ("var", 3));

                    S = ValueConvertor(bytes.Skip(68), ("VA", 3));
                    Sa = ValueConvertor(bytes.Skip(72), ("VA", 3));
                    Sb = ValueConvertor(bytes.Skip(76), ("VA", 3));
                    Sc = ValueConvertor(bytes.Skip(80), ("VA", 3));

                     Pf = ValueConvertor(bytes.Skip(84), ("", 4));
                    Pfa = ValueConvertor(bytes.Skip(88), ("", 4));
                    Pfb = ValueConvertor(bytes.Skip(92), ("", 4));
                    Pfc = ValueConvertor(bytes.Skip(96), ("", 4));

                    angle_Ua = ValueConvertor(bytes.Skip(100), ("", 3));
                    angle_Ub = ValueConvertor(bytes.Skip(104), ("", 3));
                    angle_Uc = ValueConvertor(bytes.Skip(108), ("", 3));
                    angle_Ia = ValueConvertor(bytes.Skip(112), ("", 3));
                    angle_Ib = ValueConvertor(bytes.Skip(116), ("", 3));
                    angle_Ic = ValueConvertor(bytes.Skip(120), ("", 3));

                    PosActT = ValueConvertor(bytes.Skip(124), ("W", 3));
                    RevActT = ValueConvertor(bytes.Skip(128), ("W", 3));
                    PosReactT = ValueConvertor(bytes.Skip(132), ("var", 3));
                    RevReactT = ValueConvertor(bytes.Skip(136), ("var", 3));

                    RtcBattV = ValueConvertor(bytes.Skip(140),("v",2));

                    Freq = ValueConvertor(bytes.Skip(144), ("Hz",2),2);
                    CurRate = ValueConvertor(bytes.Skip(146), ("",0),1);
                    RunMode = ValueConvertor(bytes.Skip(147), ("",0),1);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               // throw;
            }

            return false;
        }

        public string RunMode { get; set; }

        public string CurRate { get; set; }

        public string Freq { get; set; }

        private string ValueConvertor(IEnumerable<byte> dataBytes, (string Unit, int Scalar) unitAndScalar,
            int length = 4)
        {
            uint result = 0;
            string resultstring = "";
            if (length == 4)
            {
                result = BitConverter.ToUInt32(dataBytes.Take(length).Reverse().ToArray(), 0);
            }
            else if (length == 2)
            {
                result = BitConverter.ToUInt16(dataBytes.Take(length).Reverse().ToArray(), 0);
            }
            else if (length == 1)
            {
                result = dataBytes.Take(length).ToArray()[0];
            }

            if (unitAndScalar.Scalar == 0)
            {
                resultstring = (result).ToString() + unitAndScalar.Unit;
            }
            else if (unitAndScalar.Scalar == 1)
            {
                resultstring = (result * 0.1).ToString() + unitAndScalar.Unit;
            }
            else if (unitAndScalar.Scalar == 2)
            {
                resultstring = (result * 0.01).ToString() + unitAndScalar.Unit;
            }
            else if (unitAndScalar.Scalar == 3)
            {
                resultstring = (result * 0.001).ToString() + unitAndScalar.Unit;
            }
            else if (unitAndScalar.Scalar == 4)
            {
                resultstring = (result * 0.0001).ToString() + unitAndScalar.Unit;
            }

            return resultstring;
            // return (result*0.001).ToString();
        }
    }
}