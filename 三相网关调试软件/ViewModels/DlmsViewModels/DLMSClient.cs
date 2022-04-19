using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Set;
using MySerialPortMaster;

namespace 三相智慧能源网关调试软件.ViewModels.DlmsViewModels
{
    public class DlmsClient : ObservableObject
    {
        //业务层
        public Business Business { get; set; }


        #region 物理通道资源

        /// <summary>
        /// 网络资源
        /// </summary>
        public TcpServerHelper Socket { get; set; }

        /// <summary>
        /// 标识当前的Socket链接
        /// </summary>
        public Socket CurrentSocket { get; set; }

        #endregion

        protected void OnReportSnackbar(string message)
        {
            StrongReferenceMessenger.Default.Send(message, "Snackbar");
        }


        /// <summary>
        /// DlmsSettings配置相关
        /// </summary>
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }
        public TcpServerViewModel TcpServerViewModel { get; }
        public SerialPortMaster SerialPortViewModel { get; }


        /// <summary>
        /// 用于取消长时间通讯
        /// </summary>
        public RelayCommand CancelCommand { get; set; }


        public async Task<bool> InitRequest()
        {
            Business = new Business(DlmsSettingsViewModel,SerialPortViewModel,TcpServerViewModel);
            return await Business.InitRequestAsync();
        }

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


        public RelayCommand InitRequestCommand { get; set; }

        public RelayCommand ReleaseRequestCommand { get; set; }


        public DlmsClient(DlmsSettingsViewModel dlmsSettingsViewModel, TcpServerViewModel tcpServerViewModel,SerialPortMaster serialPortMaster)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            TcpServerViewModel = tcpServerViewModel;
            SerialPortViewModel = serialPortMaster;
            Socket = tcpServerViewModel.TcpServerHelper;
            Business = new Business(DlmsSettingsViewModel, SerialPortViewModel, TcpServerViewModel);
            InitRequestCommand = new RelayCommand(async () =>
            {
                await InitRequest();
            });
            ReleaseRequestCommand = new RelayCommand(async () => { await ReleaseRequest(); });
            CancelCommand = new RelayCommand(async () => { await Business.Cancel(); });
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