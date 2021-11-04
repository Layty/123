using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Set;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    /// <summary>
    /// 业务层
    /// </summary>
    public class Business
    {
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }
      public  Protocol Protocol { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Business(DlmsSettingsViewModel dlmsSettingsViewModel)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            Protocol=new Protocol(dlmsSettingsViewModel);
             CancellationTokenSource = new CancellationTokenSource();
        }
        /// <summary>
        /// 业务层的初始化 ，内部调用协议层的SNRM
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitRequestAsync()
        {
         return await  Protocol.InitAsync();
           
        }
        public async Task<bool> ReleaseRequestAsync(bool force = true)
        {
          return await Protocol.ReleaseRequestAsync(force);  
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
         GetRequestType getRequestType = GetRequestType.Normal)
        {
          return   await  Protocol.   GetRequestAndWaitResponse(cosemAttributeDescriptor, getRequestType);
        }
        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
         CosemAttributeDescriptor cosemAttributeDescriptor, GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Protocol.GetRequestAndWaitResponseArray(cosemAttributeDescriptor, getRequestType);
        }
        public async Task<GetResponse> GetRequestAndWaitResponse(
          CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
          GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Protocol.GetRequestAndWaitResponse(cosemAttributeDescriptorWithSelection, getRequestType);
        }
        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
          CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
          GetRequestType getRequestType = GetRequestType.Normal)
        {
            return await Protocol.GetRequestAndWaitResponseArray(cosemAttributeDescriptorWithSelection, getRequestType);
        }

        public async Task<SetResponse> SetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
           DlmsDataItem value)
        {
            return await Protocol.SetRequestAndWaitResponse(cosemAttributeDescriptor, value);
        }
        public async Task<byte[]> ActionRequestAndWaitResponseWithByte(CosemMethodDescriptor cosemMethodDescriptor,
         DlmsDataItem dlmsDataItem)
        {
            return await Protocol.ActionRequestAndWaitResponseWithByte(cosemMethodDescriptor, dlmsDataItem);
        }
        public async Task<Response> ActionRequestAndWaitResponse(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem dlmsDataItem)
        {
            return await Protocol.ActionRequestAndWaitResponse(cosemMethodDescriptor, dlmsDataItem);
        }



        public async Task Cancel() {
            Protocol.CancellationTokenSource.Cancel();
            await Task.Delay(2000);
            Protocol.CancellationTokenSource = new CancellationTokenSource();
        }
      
    }
}

