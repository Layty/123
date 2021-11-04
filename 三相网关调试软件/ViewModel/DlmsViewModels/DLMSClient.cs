using CommonServiceLocator;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Release;
using MyDlmsStandard.ApplicationLay.Set;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Ber;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;
using MyDlmsStandard.Wrapper;
using MySerialPortMaster;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
  

    public class DlmsClient : ObservableObject
    {
        //业务层
        Business Business;
        #region 协议层资源
     
        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }

        #endregion
        #region 物理通道资源

        /// <summary>
        /// 串口资源
        /// </summary>
        private SerialPortMaster PortMaster { get; set; }

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

        public bool IsAuthenticationRequired { get; set; }

        /// <summary>
        /// DlmsSettings配置相关
        /// </summary>
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }


        /// <summary>
        /// 用于取消长时间通讯
        /// </summary>
        public RelayCommand CancelCommand { get; set; }


   


    
        public async Task<bool> InitRequest()
        {
            Business = new Business(DlmsSettingsViewModel);
             return await Business.InitRequestAsync();
        }

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptor cosemAttributeDescriptor, GetRequestType getRequestType = GetRequestType.Normal)
        {
       return  await  Business.GetRequestAndWaitResponseArray(cosemAttributeDescriptor, getRequestType);
      
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Business.GetRequestAndWaitResponse(cosemAttributeDescriptor,getRequestType);
         
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


        public async Task<byte[]> ActionRequestAndWaitResponse(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem dlmsDataItem)
        {
            return await Business.ActionRequestAndWaitResponseWithByte(cosemMethodDescriptor, dlmsDataItem);
        }


  


        public async Task<bool> ReleaseRequest(bool force = true)
        {
          return await  Business.ReleaseRequestAsync(force);   
        }

  

        public RelayCommand InitRequestCommand { get; set; }

        public RelayCommand ReleaseRequestCommand { get; set; }


        public DlmsClient(DlmsSettingsViewModel dlmsSettingsViewModel, TcpServerViewModel tcpServerViewModel, SerialPortViewModel serialPortViewModel)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            Socket = tcpServerViewModel.TcpServerHelper;
            PortMaster = serialPortViewModel.SerialPortMaster;

            Hdlc46FrameBase = new Hdlc46FrameBase(DlmsSettingsViewModel.ServerAddress,
                (byte)DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);

            InitRequestCommand = new RelayCommand(async () => { await InitRequest(); });
            ReleaseRequestCommand = new RelayCommand(async () => { await ReleaseRequest(); });

            CancelCommand = new RelayCommand(async () =>
            {
                await Business.Cancel();
            });
        }
        //public DlmsClient()
        //{
        //    DlmsSettingsViewModel = ServiceLocator.Current.GetInstance<DlmsSettingsViewModel>();
        //    Socket = ServiceLocator.Current.GetInstance<TcpServerViewModel>().TcpServerHelper;
        //    PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;

        //    Hdlc46FrameBase = new Hdlc46FrameBase(DlmsSettingsViewModel.ServerAddress,
        //        (byte)DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);

        //    InitRequestCommand = new RelayCommand(async () => { await InitRequest(); });
        //    ReleaseRequestCommand = new RelayCommand(async () => { await ReleaseRequest(); });

        //    CancelCommand = new RelayCommand(async () =>
        //    {
        //       await Business.Cancel();
        //    });
        //}

        /// <summary>
        /// 进入基表的升级模式，写256
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> SetEnterUpGradeMode()
        {
            //TODO 

            return PortMaster.SendAndReceiveReturnDataAsync(Hdlc46FrameBase.SetEnterUpGradeMode(256));
        }
    }
}