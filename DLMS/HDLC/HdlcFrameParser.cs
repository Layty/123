using System;
using Gurux.DLMS;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class HdlcFrameParser
    {
        internal static void ParseSnrmUaResponse(GXByteBuffer data, GXDLMSSettings settings)
        {
            if (data.Size == 0)
            {
                settings.Limits.MaxInfoRX = 128;
                settings.Limits.MaxInfoTX = 128;
                settings.Limits.WindowSizeRX = 1;
                settings.Limits.WindowSizeTX = 1;
                return;
            }
            data.GetUInt8();
            data.GetUInt8();
            data.GetUInt8();
            while (data.Position < data.Size)
            {
                HDLCInfo.InfoTag uInt = (HDLCInfo.InfoTag)data.GetUInt8();
                object value;
                switch (data.GetUInt8())
                {
                    case 1:
                        value = data.GetUInt8();
                        break;
                    case 2:
                        value = data.GetUInt16();
                        break;
                    case 4:
                        value = data.GetUInt32();
                        break;
                    default:
                        throw new GXDLMSException("Invalid Exception.");
                }
                switch (uInt)
                {
                    case HDLCInfo.InfoTag.MaxInfoTx:
                        settings.Limits.MaxInfoRX = Convert.ToUInt16(value);
                        break;
                    case HDLCInfo.InfoTag.MaxInfoRx:
                        settings.Limits.MaxInfoTX = Convert.ToUInt16(value);
                        //if (settings.Limits.UseFrameSize)
                        //{
                        //    byte[] hdlcAddressBytes = GetHdlcAddressBytes(settings.ClientAddress, 0);
                        //    settings.Limits.MaxInfoTX += (ushort)(10 + hdlcAddressBytes.Length);
                        //}
                        break;
                    case HDLCInfo.InfoTag.WindowSizeTx:
                        settings.Limits.WindowSizeRX = Convert.ToByte(value);
                        break;
                    case HDLCInfo.InfoTag.WindowSizeRx:
                        settings.Limits.WindowSizeTX = Convert.ToByte(value);
                        break;
                    default:
                        throw new GXDLMSException("Invalid UA response.");
                }
            }
        }
        private static bool CheckFlagIs7E(byte[] inputUaFrameBytes)
        {
            bool flag = inputUaFrameBytes[0] != 126 && inputUaFrameBytes[inputUaFrameBytes.Length - 1] != 126;
            return !flag;
        }


        public static bool CheckUaFrameData(byte[] inputUaFrameBytes)
        {
            bool flag = inputUaFrameBytes.Length == 0;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = inputUaFrameBytes[0] != 126 && inputUaFrameBytes[inputUaFrameBytes.Length - 1] != 126;
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    bool flag3 = inputUaFrameBytes[8] == 115;
                    result = flag3;
                }
            }

            return result;
        }


        public static bool CheckIFrame(byte[] frameBytes)
        {
            bool flag = frameBytes.Length == 0;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = frameBytes[0] != 126 && frameBytes[frameBytes.Length - 1] != 126;
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    bool flag3 = (frameBytes[8] & 1) == 1;
                    result = !flag3;
                }
            }

            return result;
        }


        public static bool CheckDmFrameData(byte[] inputUaFrameBytes)
        {
            bool flag = inputUaFrameBytes.Length == 0;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = inputUaFrameBytes[0] != 126 && inputUaFrameBytes[inputUaFrameBytes.Length - 1] != 126;
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    bool flag3 = inputUaFrameBytes[8] == 31;
                    result = flag3;
                }
            }

            return result;
        }


        public static string CheckData(byte[] dataBytes)
        {
            return "";
        }
    }
}