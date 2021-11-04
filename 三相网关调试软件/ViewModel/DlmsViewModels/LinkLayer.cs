using CommonServiceLocator;
using MyDlmsStandard;
using MySerialPortMaster;
using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    /// <summary>
    /// 物理层,只需要处理数据的发送即可
    /// </summary>
    public class LinkLayer
    {
        #region 物理通道资源

        /// <summary>
        /// 串口资源
        /// </summary>
        public SerialPortMaster PortMaster { get; set; }

        /// <summary>
        /// 网络资源
        /// </summary>
        public TcpServerHelper Socket { get; set; }

        /// <summary>
        /// 标识当前的Socket链接
        /// </summary>
        public Socket CurrentSocket { get; set; }

        #endregion
        public ChanelType CommunicationType { get; set; }
        private readonly SerialPortConfigCaretaker _caretaker = new SerialPortConfigCaretaker();
        public LinkLayer()
        {
            PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;
            InitSerialPortParams(PortMaster);
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serialPortMaster"></param>
        private void InitSerialPortParams(SerialPortMaster serialPortMaster)
        {
            serialPortMaster.DataBits = 8;
            serialPortMaster.StopBits = StopBits.One;
            serialPortMaster.Parity = Parity.None;
        }   /// <summary>
            /// 初始化21E的串口实例
            /// </summary>
        public void Init21ESerialPort(int StartBaud, int _negotiateBaud)
        {
            _negotiateBaud = PortMaster.BaudRate;

            PortMaster.BaudRate = StartBaud;
            PortMaster.DataBits = 7;
            PortMaster.StopBits = StopBits.One;
            PortMaster.Parity = Parity.Even;
        }
        /// <summary>
        /// 备份当前串口参数，用于后续恢复
        /// </summary>
        public void BackupPortPara()
        {
            var memento = PortMaster.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            PortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = false;
            PortMaster.SerialPortLogger.IsReceiveFormat16 = false;
        }
        /// <summary>
        /// 恢复备份的串口参数
        /// </summary>
        public void LoadBackupPortPara()
        {
            PortMaster.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            PortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = true;
            PortMaster.SerialPortLogger.IsReceiveFormat16 = true;
        }

        #region 发送数据 //协议层=>物理层


        //public async Task<byte[]> SendAsync(IToPduStringInHex sendHexString)
        //{
        //    return await SendAsync(sendHexString.ToPduStringInHex());
        //}

        public async Task<byte[]> SendAsync(string sendHexString)
        {
            return await SendAsync(sendHexString.StringToByte());
        }  /// <summary>
           /// 如何选择物理通道进行发送数据,得到返回的报文
           /// </summary>
           /// <param name="sendBytes"></param>
           /// <returns></returns>
        public async Task<byte[]> SendAsync(byte[] sendBytes)
        {
            var returnBytes = new byte[] { };
            try
            {
                if (CommunicationType == ChanelType.SerialPort)
                {
                    returnBytes = await PortMaster.SendAndReceiveReturnDataAsync(sendBytes);
                }
                else if (CommunicationType == ChanelType.FrontEndProcess)
                {
                    returnBytes = await Socket.SendDataToClientAndWaitReceiveDataAsync(CurrentSocket, sendBytes);
                }
            }
            catch (Exception e)
            {
                // OnReportSnackbar(e.Message);
            }


            return returnBytes;
        }
        #endregion


    }
}

