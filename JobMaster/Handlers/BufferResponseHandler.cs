using DotNetty.Transport.Channels;
using JobMaster.Models;
using JobMaster.Services;
using JobMaster.ViewModels;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.Get;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JobMaster.Handlers
{
    public class BufferResponseHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;

        private readonly IProtocol Protocol;

        public static Dictionary<string, List<GetResponse>> ResponsesBuffer = new ();
        public static Dictionary<string, List<Energy>> ResponsesBufferData = new ();

        public BufferResponseHandler(NetLoggerViewModel logger, IProtocol protocol)
        {
            _logger = logger;
            _logger.LogTrace("BufferResponseHandler 实例化成功");
         
            Protocol = protocol;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                if (!ResponsesBuffer.ContainsKey(context.Channel.RemoteAddress.ToString()))
                {
                    ResponsesBuffer[context.Channel.RemoteAddress.ToString()] = new List<GetResponse>();
                }

                var result = Protocol.TakeReplyApduFromFrame(bytes);

                var re = AppProtocolFactory.CreateGetResponse(result);
                if (re?.GetResponseNormal != null)
                {
                    try
                    {
                        List<GetResponse> responses = new List<GetResponse>();
                        responses.Add(re);
                        ResponsesBuffer[context.Channel.RemoteAddress.ToString()] = responses;
                        var Energies = new List<Energy>();
                        var dlmsStructures = MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage.CosemProfileGeneric
                            .ParseBuffer(responses);
                        if (dlmsStructures != null)
                        {
                            foreach (var item in dlmsStructures)
                            {
                                var dataItems = item.Items;
                                var clock = new CosemClock();
                                string dt = dataItems[0].Value.ToString();
                                var b = clock.DlmsClockParse(dt.StringToByte());
                                if (b)
                                {
                                    EnergyCaptureObjects energyCaptureObjects = new ()
                                    {
                                        DateTime = clock.ToDateTime(),
                                        ImportActiveEnergyTotal = dataItems[1].ValueString,
                                        ImportActiveEnergyT1 = dataItems[2].ValueString,
                                        ImportActiveEnergyT2 = dataItems[3].ValueString,
                                        ImportActiveEnergyT3 = dataItems[4].ValueString,
                                        ImportActiveEnergyT4 = dataItems[5].ValueString,
                                        ExportActiveEnergyTotal = dataItems[6].ValueString,
                                        ImportReactiveEnergyTotal = dataItems[7].ValueString,
                                        ExportReactiveEnergyTotal = dataItems[8].ValueString
                                    };

                                    Energies.Add(new Energy()
                                    {
                                        EnergyData = JsonConvert.SerializeObject(energyCaptureObjects),
                                        Id = Guid.NewGuid(),
                                        DateTime = clock.ToDateTime(),
                                        //MeterId = t.MeterId
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //else if (re?.GetResponseWithDataBlock != null)
                //{
                //    if (re.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "00")
                //    {
                //        ResponsesBuffer[context.Channel.RemoteAddress.ToString()].Add(re);
                //        var blockNumber = re.GetResponseWithDataBlock.DataBlockG.BlockNumber;
                //        var getRequestNext = AppProtocolFactory.CreateGetRequestNext(blockNumber);
                //        var next = dlmsClient.Business.Protocol.BuildFinalSendData(ProtocolInterfaceType.WRAPPER, getRequestNext);
                //        context.WriteAndFlushAsync(next);
                //    }
                //    else if (re.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "01")
                //    {
                //        ResponsesBuffer[context.Channel.RemoteAddress.ToString()].Add(re);
                //    }

                //}
                else
                {
                    ResponsesBuffer[context.Channel.RemoteAddress.ToString()] = null;
                    context.FireChannelRead(bytes);
                }
            }
        }
    }
}