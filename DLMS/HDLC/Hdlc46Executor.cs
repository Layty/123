using System;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using 三相智慧能源网关调试软件.Model;
using Task = System.Threading.Tasks.Task;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class Hdlc46Executor
    {
        public static HdlcFrameMaker HdlcFrameMaker { get; set; }
        public GXDLMSClient Client { get; set; }

        private readonly MySerialPort _port;

        private void InitSerialPortParams(MySerialPort serialPort)
        {
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
        }


        public Hdlc46Executor(MySerialPort mySerialPort, HdlcFrameMaker hdlc46Frame)
        {
            _port = mySerialPort;
            InitSerialPortParams(_port);
            HdlcFrameMaker = hdlc46Frame;
        }


        public Hdlc46Executor(MySerialPort mySerialPort, byte[] desAddr, string password = "33333333",
            byte sourceAddr = 1)
        {
            _port = mySerialPort;
            InitSerialPortParams(_port);
            HdlcFrameMaker = new HdlcFrameMaker(desAddr, password, sourceAddr);

            Client = new GXDLMSClient();
            Client.UseLogicalNameReferencing = true;
            Client.InterfaceType = InterfaceType.HDLC;
            Client.ClientAddress = 1;
         
            Client.ServerAddress = 1;
            Client.Settings.Limits.UseFrameSize = true;
            Client.Settings.Limits.MaxInfoRX = 1024;
            Client.Settings.Limits.MaxInfoTX = 1024;
            Client.Settings.Limits.WindowSizeTX = 1;
            Client.Settings.Limits.WindowSizeRX = 7;
        }

        public Hdlc46Executor(string fileName, MySerialPort mySerialPort, byte[] desAddr, string password = "33333333",
            byte sourceAddr = 1) : this(mySerialPort, desAddr, password, sourceAddr)
        {
            // string excelFileName = Settings.Default.DLMSMeterConfigExcelFile;
            //  ExcelHelper excelHelper = new ExcelHelper(excelFileName);
            //  this.HdlcDataTable = excelHelper.GetExcelDataTable(fileName);
        }




        public Task<byte[]> ExecuteApp(byte[] obisframe)
        {
            return _port.SendAndReceiveReturnDataAsync(obisframe);
        }

        public Task<bool> ExecuteHdlcSNRMRequest()
        {
            var t = Task.Run(() =>
            {
                var r = _port.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SNRMRequest()).Result;
                if (r == null)
                {
                    return false;
                }

                var buff = r.Skip(8).ToArray();
                var buff1 = new GXByteBuffer(buff.Take(buff.Length - 3).ToArray());
                Client = new GXDLMSClient();
                Client.ParseUAResponse(buff1);
             
                _port.SendAndReceiveDataCollections = "ParseUAResponse";
                return true;
            });
            return t;
        }


        public Task ExecuteHdlcDisConnectRequest()
        {
            return _port.SendAndReceiveReturnDataAsync(HdlcFrameMaker.DisconnectRequest());
        }

        public Task ExecuteHdlcComm(Func<byte[]> func)
        {
            return _port.SendAndReceiveReturnDataAsync(func());
        }

    
    
        public Task ExecuteHdlcComm(byte[] dataBytes)
        {
            return _port.SendAndReceiveReturnDataAsync(dataBytes);
        }
    }
}