using System;
using System.Threading.Tasks;
using CommonServiceLocator;
using Gurux.DLMS.ManufacturerSettings;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.HDLC;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    /// <summary>
    /// 接口类base本身没有明确规定，它只包含一个属性“逻辑名”
    /// </summary>
    public abstract class DLMSObject
    {
        public GXAttributeCollection Attributes { get; set; }
        public GXAttributeCollection MethodAttributes { get; set; }
        public string Description { get; set; }

        public virtual string LogicalName { get; set; }

        public virtual ObjectType ObjectType { get; set; }
        public ushort ShortName { get; set; }


        public object Tag { get; set; }
        public virtual byte Version { get; set; } = 0;

        public static void ValidateLogicalName(string ln)
        {
            if (ln.Split('.').Length != 6)
            {
                throw new Exception("Invalid Logical Name.");
            }
        }

        public SerialPortViewModel SerialPortViewModel = ServiceLocator.Current.GetInstance<SerialPortViewModel>();
        public DlmsViewModel DlmsViewModel = ServiceLocator.Current.GetInstance<DlmsViewModel>();

        protected Task<byte[]> GetAttributeData(byte attrId)
        {
            var msg = DlmsViewModel.HdlcFrameMaker.GetRequest(this, attrId);
            return SerialPortViewModel.SerialPortMasterModel.SendAndReceiveReturnDataAsync(msg);
        }

        protected async void SetAttributeData(byte attrId, DLMSDataItem dlmsDataItem)
        {
            var msg = DlmsViewModel.HdlcFrameMaker.SetRequest(this, attrId, dlmsDataItem);
            await SerialPortViewModel.SerialPortMasterModel.SendAndReceiveReturnDataAsync(msg);
        }

        protected async void ActionExecute(byte methodIndex, DLMSDataItem dlmsDataItem)
        {
            var msg = DlmsViewModel.HdlcFrameMaker.ActionRequest(this, methodIndex, dlmsDataItem);
            await SerialPortViewModel.SerialPortMasterModel.SendAndReceiveReturnDataAsync(msg);
        }
    }
}