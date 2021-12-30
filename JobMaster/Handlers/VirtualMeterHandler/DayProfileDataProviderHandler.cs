using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using JobMaster.Models;
using JobMaster.Services;
using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.Axdr;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JobMaster.Handlers
{
    public class DayProfileDataProviderHandler : ChannelHandlerAdapter, INotifyPropertyChanged
    {
        private readonly NetLoggerViewModel _logger;
        private readonly IProtocol Protocol;
        private readonly EnergyCaptureObjects1 energyCapture;

        public DateTime DateTime
        {
            get => _DateTime;
            set
            {
                _DateTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime _DateTime = DateTime.Now;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public DayProfileDataProviderHandler(NetLoggerViewModel logger, IProtocol protocol, EnergyCaptureObjects1 energyCapture)
        {
            _logger = logger;
            Protocol = protocol;
            this.energyCapture = energyCapture;
            _logger.LogTrace("DayProfileDataProviderHandler 实例化成功");
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = Protocol.TakeReplyApduFromFrame(bytes)
                                   .ByteToString();
                var getRequest = new GetRequest();
                if (getRequest.PduStringInHexConstructor(ref result))
                {
                    //属于GetRequest请求,再判断OBIS 
                    //00 01 00 01 00 01 00 0D C0 01 C1 00 07 01 00 62 01 01 FF 03 00 日冻结捕获对象
                    if (getRequest?.GetRequestNormal != null)
                    {
                        var attr = getRequest.GetRequestNormal.AttributeDescriptor?.ToPduStringInHex();
                        if (attr == "00 07 01 00 62 01 01 FF 03".Replace(" ", ""))
                        {
                            //此处返回日冻结的捕获对象
                            //00 01 00 01 00 01 00 A8 C4 01 C1 00 01 09 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 01 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 02 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 03 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 04 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 02 08 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 03 08 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 04 08 00 FF 0F 02 12 00 00
                            var tcap = Unpooled.Buffer();
                            var dayCapture = "00 01 00 01 00 01 00 A8 C4 01 C1 00 01 09 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 01 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 02 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 03 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 01 08 04 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 02 08 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 03 08 00 FF 0F 02 12 00 00 02 04 12 00 03 09 06 01 00 04 08 00 FF 0F 02 12 00 00";
                            tcap.WriteBytes(dayCapture.StringToByte());
                            context.WriteAndFlushAsync(tcap);
                            return;
                        }
                        var buffer = getRequest.GetRequestNormal.AttributeDescriptorWithSelection?.ToPduStringInHex();
                        //00 01 00 01 00 01 00 40 C0 01 C1 00 07 01 00 62 01 01 FF 02 01 01 02 04 02 04 12 00 08 09 06 00 00 01 00 00 FF 0F 02 12 00 00 09 0C 07 E5 0C 1D 03 00 00 00 00 80 00 00 09 0C 07 E5 0C 1E 04 08 35 2B 00 80 00 00 01 00
                        if (buffer != null && buffer.Contains("07 01 00 62 01 01 FF 02".Replace(" ", "")))
                        {
                            //00 01 00 01 00 01 00 3E C4 01 C1 00 01 01 02 09 09 0C 07 E5 0C 1E 04 00 00 00 FF 80 00 00 06 00 00 00 00 06 00 00 00 00 06 00 00 00 00 06 00 00 00 00 06 00 00 00 00 06 00 00 00 00 06 00 00 00 00 06 00 00 00 00
                            var tcap = Unpooled.Buffer();
                            var dt = new DlmsDataItem(DataType.OctetString, new CosemClock(energyCapture.DateTime).GetDateTimeBytes().ByteToString());
                            var ImportActiveEnergyTotal = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ImportActiveEnergyTotal)).ToPduStringInHex());
                            var ImportActiveEnergyT1 = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ImportActiveEnergyT1)).ToPduStringInHex());
                            var ImportActiveEnergyT2 = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ImportActiveEnergyT2)).ToPduStringInHex());
                            var ImportActiveEnergyT3 = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ImportActiveEnergyT3)).ToPduStringInHex());
                            var ImportActiveEnergyT4 = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ImportActiveEnergyT4)).ToPduStringInHex());
                            var ExportActiveEnergyTotal = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ExportActiveEnergyTotal)).ToPduStringInHex());
                            var ImportReactiveEnergyTotal = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ImportReactiveEnergyTotal)).ToPduStringInHex());
                            var ExportReactiveEnergyTotal = new DlmsDataItem(DataType.UInt32, new AxdrIntegerUnsigned32(uint.Parse(energyCapture.ExportReactiveEnergyTotal)).ToPduStringInHex());

                            var dayCapture = $"00 01 00 01 00 01 00 3E C4 01 C1 00 01 01 02 09 " +
                                $"{dt.ToPduStringInHex()}" +
                                $" {ImportActiveEnergyTotal.ToPduStringInHex()}" +
                                $"{ImportActiveEnergyT1.ToPduStringInHex()}" +
                                $"{ImportActiveEnergyT2.ToPduStringInHex()}" +
                                $"{ImportActiveEnergyT3.ToPduStringInHex()}" +
                                $"{ImportActiveEnergyT4.ToPduStringInHex()}" +
                                $"{ExportActiveEnergyTotal.ToPduStringInHex()}" +
                                $"{ImportReactiveEnergyTotal.ToPduStringInHex()}" +
                                $"{ExportReactiveEnergyTotal.ToPduStringInHex()}";
                            tcap.WriteBytes(dayCapture.StringToByte());
                            context.WriteAndFlushAsync(tcap);
                            return;
                        }
                    }

                    //var getResponse = new GetResponse()
                    //{
                    //    GetResponseNormal = new GetResponseNormal()
                    //    {
                    //        Result = new MyDlmsStandard.ApplicationLay.GetDataResult()
                    //        {

                    //            DataAccessResult = new MyDlmsStandard.Axdr.AxdrIntegerUnsigned8("00"),
                    //            Data = new MyDlmsStandard.ApplicationLay.DlmsDataItem()
                    //            {
                    //                DataType = MyDlmsStandard.ApplicationLay.ApplicationLayEnums.DataType.Array,
                    //            },
                    //        },
                    //    }
                    //};
                    //var response = getResponse.ToPduStringInHex();
                    //var t = Unpooled.Buffer();
                    ////t.WriteBytes(response.StringToByte());
                    //context.WriteAndFlushAsync(t);

                }
                else
                {
                    //跳转给下一个handler处理
                    context.FireChannelRead(bytes);

                }
            }
        }
    }
}