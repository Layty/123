namespace MyDlmsStandard.HDLC
{
    public class HdlcFrameParser
    {
        //internal static void ParseSnrmUaResponse(byte[] data, MyDLMSSettings settings)
        //{
        //    // 7E A0 20 03 03 73 F0 2E 81 80 14 05 02 01 54 06 02 01 54 07 04 00 00 00 01 08 04 00 00 00 01 05 29 7E
        //    if (data.Length == 0)
        //    {
        //        settings.DlmsInfo.SetDefaultValue();
        //        return;
        //    }

        //    var count = data[3];
        //    if (data.Length == (1 + 1 + count + 1))
        //    {
        //        settings.DlmsInfo.SetDefaultValue();
        //        return;
        //    }

        //    var index = settings.ServerAddressSize + 1 + 1 + 2;
        //    var infoBytes = data.Skip(3 + index).Take(count - 3 - index - 1);
        //    var removeFormatIdentifierAndGroupIdentifier = infoBytes.Skip(3).ToArray();
        //    if (removeFormatIdentifierAndGroupIdentifier[0] != (removeFormatIdentifierAndGroupIdentifier.Length + 1))
        //    {
        //        return;
        //    }

        //    var realMsg = removeFormatIdentifierAndGroupIdentifier.Skip(1).ToArray();


        
        //    {
        //       // DLMSInfo.InfoTag uInt = (DLMSInfo.InfoTag) data.GetUInt8();
        //        for (int i = 0; i < realMsg.Length; i++)
        //        {
        //            DLMSInfo.InfoTag uInt = (DLMSInfo.InfoTag) data[i];
        //            object value;
        //            switch (uInt)
        //            {
        //                case DLMSInfo.InfoTag.MaxInfoTx:
        //                    i++;
        //                   var le= data.Skip(i).Take(1).ToArray()[0];
                          
        //                   // switch (le)
        //                   //{
        //                   //    case 1:
        //                   //        value = data.GetUInt8();
        //                   //        break;
        //                   //    case 2:
        //                   //        value = data.GetUInt16();
        //                   //        break;
        //                   //    case 4:
        //                   //        value = data.GetUInt32();
        //                   //        break;
        //                   //    default:
        //                   //        throw new GXDLMSException("Invalid Exception.");
        //                   // }
        //                    settings.DlmsInfo.ReceiveMaxInfoValue = Convert.ToUInt16(value);
        //                    break;
        //                case DLMSInfo.InfoTag.MaxInfoRx:
        //                    settings.DlmsInfo.TransmitMaxInfoValue = Convert.ToUInt16(value);
        //                    //if (settings.Limits.UseFrameSize)
        //                    //{
        //                    //    byte[] hdlcAddressBytes = GetHdlcAddressBytes(settings.ClientAddress, 0);
        //                    //    settings.Limits.MaxInfoTX += (ushort)(10 + hdlcAddressBytes.Length);
        //                    //}
        //                    break;
        //                case DLMSInfo.InfoTag.WindowSizeTx:
        //                    settings.DlmsInfo.ReceiveMaxWindowSize = Convert.ToByte(value);
        //                    break;
        //                case DLMSInfo.InfoTag.WindowSizeRx:
        //                    settings.DlmsInfo.TransmitMaxWindowSize = Convert.ToByte(value);
        //                    break;
        //                default:
        //                    throw new GXDLMSException("Invalid UA response.");
        //            }
        //        }
               
        //        object value;
              
        //    }
        //}

        private static void getData(byte input)
        {

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