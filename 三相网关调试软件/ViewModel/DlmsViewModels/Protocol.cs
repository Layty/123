using System.Threading.Tasks;
using MyDlmsStandard;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;
using MyDlmsStandard.HDLC.IEC21EMode;
using MyDlmsStandard.Wrapper;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    /// <summary>
    ///协议层
    /// </summary>
    public class Protocol
    {
        public InterfaceType InterfaceType { get; set; }
        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public WrapperFrame WrapperFrame { get; set; }
        public EModeFrame EModeFrame { get; set;  }

        public StartProtocolType StartProtocolType {  get; set; }
        public SendData sendData { get; set; }
        public Protocol(DlmsSettingsViewModel dlmsSettingsViewModel)
        {
            InterfaceType = dlmsSettingsViewModel.InterfaceType;
            StartProtocolType = dlmsSettingsViewModel.StartProtocolType;
            Hdlc46FrameBase = new Hdlc46FrameBase(dlmsSettingsViewModel.ServerAddress, (byte)dlmsSettingsViewModel.ClientAddress, dlmsSettingsViewModel.DlmsInfo);
            var wrapperHeader = new WrapperHeader()
            {
                Version = new AxdrIntegerUnsigned16("1"),
                SourceAddress = new AxdrIntegerUnsigned16(dlmsSettingsViewModel.ClientAddress.ToString("X4")),
                DestAddress = new AxdrIntegerUnsigned16(dlmsSettingsViewModel.ServerAddress.ToString("X4")),
            }; WrapperFrame = new WrapperFrame()
            {
                WrapperHeader= wrapperHeader
            };
            EModeFrame =new EModeFrame(dlmsSettingsViewModel.NegotiateBaud);
        }





        /// <summary>
        /// 根据协议类型+APDU 返回对应报文
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="Apdu"></param>
        /// <returns></returns>
        public  byte[] BuildApduData(IToPduStringInHex Apdu)
        {
           // byte[] bytes = { };
            if (InterfaceType == InterfaceType.HDLC)
            {
                Hdlc46FrameBase.Apdu = Apdu.ToPduStringInHex().StringToByte();
                sendData = new SendData(Hdlc46FrameBase);
            }
            else if (InterfaceType == InterfaceType.WRAPPER)
            {
                WrapperFrame.WrapperBody.DataBytes = Apdu.ToPduStringInHex().StringToByte();
                sendData = new SendData(WrapperFrame);
            }
         
            return sendData.ToPduStringInHex().StringToByte();
         
        }
        /// <summary>
        /// 从返回的报文中 解析apdu数据，并返回
        /// </summary>
        /// <param name="replyApduData"></param>
        /// <returns></returns>
        public byte[] ParseReplyApduData( byte[] replyApduData)
        {
            if (replyApduData == null || replyApduData.Length == 0)
            {
              //  OnReportSnackbar("未收到响应帧");
                return null;
            }

            var returnPduBytes = new byte[] { }; 
            if (InterfaceType == InterfaceType.HDLC)
            {
                var pstring = replyApduData.ByteToString();
                if (Hdlc46FrameBase.PduStringInHexConstructor(ref pstring))
                {
                    returnPduBytes = Hdlc46FrameBase.Apdu;
                }
            }
            else if (InterfaceType == InterfaceType.WRAPPER)
            {
                //47协议来解析
                WrapperFrame wrapperFrame = new WrapperFrame();
                string parseHexString = replyApduData.ByteToString();

                if (wrapperFrame.PduStringInHexConstructor(ref parseHexString))
                {
                    returnPduBytes = wrapperFrame.WrapperBody.DataBytes;
                }
            }
           
            return returnPduBytes;
        }
    }
}

