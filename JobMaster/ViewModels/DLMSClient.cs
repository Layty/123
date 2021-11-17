using DotNetty.Transport.Channels;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Set;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace JobMaster.ViewModels
{
    public class DlmsClient : BindableBase
    {
        //业务层
        public Business Business { get; set; }

        /// <summary>
        /// DlmsSettings配置相关
        /// </summary>
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }
        public FrontEndProcessorViewModel TcpServerViewModel { get; }
        public MainServerViewModel MainServerViewModel { get; set; }
        public SerialPortViewModel SerialPortViewModel { get; set; }
        /// <summary>
        /// 用于取消长时间通讯
        /// </summary>
        public DelegateCommand CancelCommand { get; set; }

        public DelegateCommand InitRequestCommand { get; set; }

        public DelegateCommand ReleaseRequestCommand { get; set; }

        public async Task<bool> InitRequest()
        {
            Business = new Business(DlmsSettingsViewModel, SerialPortViewModel, TcpServerViewModel);
            //  Business=new Business(DlmsSettingsViewModel, SerialPortViewModel, TcpServerViewModel);
            return await Business.InitRequestAsync();
        }
        //public async Task InitRequestNetty(IChannelHandlerContext context)
        //{
        //    Business = new Business(DlmsSettingsViewModel, SerialPortViewModel, context);
        //    await Business.InitRequestAsyncNetty(context);
        //}

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptor cosemAttributeDescriptor, GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Business.GetRequestAndWaitResponseArray(cosemAttributeDescriptor, getRequestType);
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Business.GetRequestAndWaitResponse(cosemAttributeDescriptor, getRequestType);
        }
        //public async Task GetRequestAndWaitResponseNetty(CosemAttributeDescriptor cosemAttributeDescriptor, IChannelHandlerContext context,
        // GetRequestType getRequestType = GetRequestType.Normal)
        //{
        //    Business = new Business(DlmsSettingsViewModel, SerialPortViewModel, context);
        //    await Business.GetRequestAndWaitResponseNetty(cosemAttributeDescriptor, getRequestType);
        //}

        public async Task<GetResponse> GetRequestAndWaitResponse(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Business.GetRequestAndWaitResponse(cosemAttributeDescriptorWithSelection, getRequestType);
        }

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Business.GetRequestAndWaitResponseArray(cosemAttributeDescriptorWithSelection, getRequestType);
        }
        //public async Task GetRequestAndWaitResponseArrayNetty(
        //  CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection, IChannelHandlerContext context,
        //  GetRequestType getRequestType = GetRequestType.Normal)
        //{
        //    Business = new Business(DlmsSettingsViewModel, SerialPortViewModel, context);
        //    await Business.GetRequestAndWaitResponseArrayNetty(cosemAttributeDescriptorWithSelection, getRequestType);
        //}

        public async Task<SetResponse> SetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            DlmsDataItem value)
        {
            return await Business.SetRequestAndWaitResponse(cosemAttributeDescriptor, value);
        }


        public async Task<ActionResponse> ActionRequestAndWaitResponse(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem dlmsDataItem)
        {
            return await Business.ActionRequestAndWaitResponse(cosemMethodDescriptor, dlmsDataItem);
        }


        public async Task<bool> ReleaseRequest(bool force = true)
        {
            return await Business.ReleaseRequestAsync(force);
        }
      



        public DlmsClient(DlmsSettingsViewModel dlmsSettingsViewModel, SerialPortViewModel serialPortViewModel, FrontEndProcessorViewModel tcpServerViewModel)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            SerialPortViewModel = serialPortViewModel;
            TcpServerViewModel = tcpServerViewModel;

            Business = new Business(DlmsSettingsViewModel, SerialPortViewModel, TcpServerViewModel);
            InitRequestCommand = new DelegateCommand(async () =>
            {
                await InitRequest();
            });
            ReleaseRequestCommand = new DelegateCommand(async () => { await ReleaseRequest(); });
            CancelCommand = new DelegateCommand(async () => { await Business.Cancel(); });
        }


        /// <summary>
        /// 进入基表的升级模式，写256
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> SetEnterUpGradeMode()
        {
            return Business.SetEnterUpGradeMode();
        }
    }
}