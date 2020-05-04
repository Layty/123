using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using MySerialPortMaster;
using Task = System.Threading.Tasks.Task;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class Hdlc46Executor
    {
        public static HdlcFrameMaker HdlcFrameMaker { get; set; }
        public GXDLMSClient Client { get; set; }

        private readonly SerialPortMaster _portMaster;

        private void InitSerialPortParams(SerialPortMaster serialPortMaster)
        {
            serialPortMaster.DataBits = 8;
            serialPortMaster.StopBits = StopBits.One;
            serialPortMaster.Parity = Parity.None;
        }


        public Hdlc46Executor(SerialPortMaster serialPortMaster, HdlcFrameMaker hdlc46Frame)
        {
            _portMaster = serialPortMaster;
            InitSerialPortParams(_portMaster);
            HdlcFrameMaker = hdlc46Frame;
        }


        public Hdlc46Executor(SerialPortMaster serialPortMaster, byte[] desAddr, string password = "33333333",
            byte sourceAddr = 1)
        {
            _portMaster = serialPortMaster;
            InitSerialPortParams(_portMaster);
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

        public Hdlc46Executor(string fileName, SerialPortMaster serialPortMaster, byte[] desAddr, string password = "33333333",
            byte sourceAddr = 1) : this(serialPortMaster, desAddr, password, sourceAddr)
        {
            // string excelFileName = Settings.Default.DLMSMeterConfigExcelFile;
            //  ExcelHelper excelHelper = new ExcelHelper(excelFileName);
            //  this.HdlcDataTable = excelHelper.GetExcelDataTable(fileName);
        }




        public Task<byte[]> ExecuteApp(byte[] obisframe)
        {
            return _portMaster.SendAndReceiveReturnDataAsync(obisframe);
        }

        public Task<bool> ExecuteHdlcSNRMRequest()
        {
            var t = Task.Run(() =>
            {
                var r = _portMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SNRMRequest()).Result;
                if (r == null)
                {
                    return false;
                }

                var buff = r.Skip(8).ToArray();
                var buff1 = new GXByteBuffer(buff.Take(buff.Length - 3).ToArray());
                Client = new GXDLMSClient();
                Client.ParseUAResponse(buff1);
             
                _portMaster.SendAndReceiveDataCollections = "ParseUAResponse";
                return true;
            });
            return t;
        }


        public Task ExecuteHdlcDisConnectRequest()
        {
            return _portMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.DisconnectRequest());
        }

        public Task ExecuteHdlcComm(Func<byte[]> func)
        {
            return _portMaster.SendAndReceiveReturnDataAsync(func());
        }

    
    
        public Task ExecuteHdlcComm(byte[] dataBytes)
        {
            return _portMaster.SendAndReceiveReturnDataAsync(dataBytes);
        }
    }
}