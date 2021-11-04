using System;
using System.Text;

namespace FileTransmit
{

    public enum CRCType
    {
        CRC16_IBM = 0,
        CRC16_MAXIM,
        CRC16_USB,
        CRC16_MODBUS,
        CRC16_CCITT,
        CRC16_CCITT_FALSE,
        CRC16_X25,
        CRC16_XMODEM,
        CRC16_DNP,
        CRC32,
        CRC32_MPEG2,
    }

    public class DataCheck
    {
        static public CRCInfo GetCRCInfo(CRCType type)
        {
            CRCInfo param = null;
            switch (type)
            {
                case CRCType.CRC16_IBM:
                    param = new CRCInfo(0x8005, 0x0000, true, true, 0x0000);
                    break;
                case CRCType.CRC16_MAXIM:
                    param = new CRCInfo(0x8005, 0x0000, true, true, 0xFFFF);
                    break;

                case CRCType.CRC16_USB:
                    param = new CRCInfo(0x8005, 0xFFFF, true, true, 0xFFFF);
                    break;

                case CRCType.CRC16_MODBUS:
                    param = new CRCInfo(0x8005, 0xFFFF, true, true, 0x0000);
                    break;

                case CRCType.CRC16_CCITT:
                    param = new CRCInfo(0x1021, 0x0000, true, true, 0x0000);
                    break;

                case CRCType.CRC16_CCITT_FALSE:
                    param = new CRCInfo(0x1021, 0xFFFF, false, false, 0x0000);
                    break;

                case CRCType.CRC16_X25:
                    param = new CRCInfo(0x1021, 0xFFFF, true, true, 0xFFFF);
                    break;

                case CRCType.CRC16_XMODEM:
                    param = new CRCInfo(0x1021, 0x0000, false, false, 0x0000);
                    break;

                case CRCType.CRC16_DNP:
                    param = new CRCInfo(0x3D65, 0x0000, true, true, 0xFFFF);
                    break;

                case CRCType.CRC32:
                    param = new CRCInfo(0x04C11DB7, 0xFFFFFFFF, true, true, 0xFFFFFFFF);
                    break;
                case CRCType.CRC32_MPEG2:
                    param = new CRCInfo(0x04C11DB7, 0xFFFFFFFF, false, false, 0x00000000);
                    break;
            }

            return param;
        }

        public static uint GetCRC(CRCType type, byte[] data)
        {
            CRCInfo param;

            param = GetCRCInfo(type);
            if (type >= CRCType.CRC16_IBM && type <= CRCType.CRC16_DNP)
            {
                return GetCRC16(param, data);
            }
            else if (type >= CRCType.CRC32 && type <= CRCType.CRC32_MPEG2)
            {
                return GetCRC32(param, data);
            }
            return 0;
        }

        public static ushort GetCRC16(CRCInfo param, byte[] data)
        {
            ushort crc = (ushort)param.Init;
            ushort Poly = 0;
            ushort XorOut = (ushort)param.XorOut;

            if (param.RefIn)
            {
                for (int i = 0; i < 16; i++)
                {
                    Poly <<= 1;
                    if ((param.Poly & (1u << i)) != 0)
                    {
                        Poly |= 0x01;
                    }
                }
            }
            else
            {
                Poly = (ushort)param.Poly;
            }

            foreach (byte b in data)
            {
                UInt16 bValue;
                if (param.RefOut)
                {
                    bValue = Convert.ToUInt16(b);
                }
                else
                {
                    bValue = Convert.ToUInt16((UInt16)b << 8);
                }

                crc = Convert.ToUInt16(crc ^ bValue);
                for (int i = 0; i < 8; i++)
                {
                    if (param.RefOut)
                    {
                        if ((crc & 0x01) != 0)
                        {
                            crc >>= 1;
                            crc ^= Poly;
                        }
                        else
                        {
                            crc >>= 1;
                        }
                    }
                    else
                    {
                        if ((crc & 0x8000) != 0)
                        {
                            crc <<= 1;
                            crc ^= Poly;
                        }
                        else
                        {
                            crc <<= 1;
                        }
                    }
                }
            }

            return Convert.ToUInt16(crc ^ XorOut);
        }

        public static uint GetCRC32(CRCInfo param, byte[] data)
        {
            uint crc = param.Init;
            uint Poly = 0;
            uint XorOut = param.XorOut;

            if (param.RefIn)
            {
                for (int i = 0; i < 32; i++)
                {
                    Poly <<= 1;
                    if ((param.Poly & (1u << i)) != 0)
                    {
                        Poly |= 0x01;
                    }
                }
            }
            else
            {
                Poly = param.Poly;
            }

            foreach (byte b in data)
            {
                uint bValue;
                if (param.RefOut)
                {
                    bValue = Convert.ToUInt32(b);
                }
                else
                {
                    bValue = Convert.ToUInt32((uint)b << 24);
                }

                crc = Convert.ToUInt32(crc ^ bValue);
                for (int i = 0; i < 8; i++)
                {
                    if (param.RefOut)
                    {
                        if ((crc & 0x01) != 0)
                        {
                            crc >>= 1;
                            crc ^= Poly;
                        }
                        else
                        {
                            crc >>= 1;
                        }
                    }
                    else
                    {
                        if ((crc & 0x80000000) != 0)
                        {
                            crc <<= 1;
                            crc ^= Poly;
                        }
                        else
                        {
                            crc <<= 1;
                        }
                    }
                }
            }

            return Convert.ToUInt32(crc ^ XorOut);
        }


        public static uint GetCheckSum(byte[] data)
        {
            uint sum = 0;

            foreach (byte b in data)
            {
                sum += b;
            }

            return sum;
        }

        public static byte GetXor(byte[] data)
        {
            byte xor = 0;

            foreach (byte b in data)
            {
                xor ^= b;
            }

            return xor;
        }

    }

    public class CRCInfo
    {
        public CRCInfo()
            : this(0, 0, false, false, 0)
        {

        }
        public CRCInfo(uint poly, uint init, bool refIn, bool refOut, UInt32 xorOut)
        {
            Poly = poly;
            Init = init;
            RefIn = refIn;
            RefOut = refOut;
            XorOut = xorOut;
        }

        /// <summary>
        /// 多项式
        /// </summary>
        public uint Poly { get; set; }

        /// <summary>
        /// 初始值
        /// </summary>
        public uint Init { get; set; }

        /// <summary>
        /// 输出CRC异或值
        /// </summary>
        public uint XorOut { get; set; }

        /// <summary>
        /// 输入多项式反序
        /// </summary>
        public bool RefIn { get; set; }

        /// <summary>
        /// 输出数据反序
        /// </summary>
        public bool RefOut { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if ((Poly & 0xFFFF0000) != 0)
            {
                sb.AppendFormat("Poly:  {0:X8}\r\n", Poly);
                sb.AppendFormat("Init:  {0:X8}\r\n", Init);
                sb.AppendFormat("RefIn: {0}\r\n", RefIn ? "True" : "False");
                sb.AppendFormat("RefOut:{0}\r\n", RefOut ? "True" : "False");
                sb.AppendFormat("XorOut:{0:X8}", XorOut);
            }
            else
            {
                sb.AppendFormat("Poly:  {0:X4}\r\n", Poly);
                sb.AppendFormat("Init:  {0:X4}\r\n", Init);
                sb.AppendFormat("RefIn: {0}\r\n", RefIn ? "True" : "False");
                sb.AppendFormat("RefOut:{0}\r\n", RefOut ? "True" : "False");
                sb.AppendFormat("XorOut:{0:X4}", XorOut);
            }

            return sb.ToString();
        }

    }
}
