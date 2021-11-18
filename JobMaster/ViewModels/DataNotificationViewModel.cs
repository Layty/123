using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.DataNotification;
using MyDlmsStandard.Axdr;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobMaster.ViewModels
{
    public class CustomAlarm : DlmsStructure
    {
        public AxdrOctetStringFixed PushId { get; set; }
        public AxdrOctetString CosemLogicalDeviceName { get; set; }
        public AxdrIntegerUnsigned32 AlarmDescriptor1 { get; set; }
        public AxdrIntegerUnsigned32 AlarmDescriptor2 { get; set; }


        public new bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (base.PduStringInHexConstructor(ref pduStringInHex))
            {
                PushId = new AxdrOctetStringFixed(6);
                var pid = Items[0].Value.ToString();
                if (!PushId.PduStringInHexConstructor(ref pid)) return false;


                var deviceName = Items[1].ToPduStringInHex().Substring(2);
                CosemLogicalDeviceName = new AxdrOctetString();
                if (!CosemLogicalDeviceName.PduStringInHexConstructor(ref deviceName)) return false;
                var descriptor1 = Items[2].Value.ToString();
                AlarmDescriptor1 = new AxdrIntegerUnsigned32();
                if (!AlarmDescriptor1.PduStringInHexConstructor(ref descriptor1)) return false;
                var descriptor2 = Items[3].Value.ToString();
                AlarmDescriptor2 = new AxdrIntegerUnsigned32();
                if (!AlarmDescriptor2.PduStringInHexConstructor(ref descriptor2)) return false;

                return true;
            }


            return false;
        }
    }
    public enum AlarmType
    {
        Unknown,
        PowerOff,
        PowerOn,
        ByPass,
        烟感and水浸,
        风机控制
    }
    public class DataNotificationModel
    {

        /// <summary>
        /// 收到消息帧的时刻
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// IP地址信息
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 表号，从IP和设备绑定表中获取
        /// </summary>
        public string MeterId { get; set; }
        /// <summary>
        /// 对Dlms的DataNotification dateTime的解析 内容的DateTime
        /// </summary>
        public string AlarmDateTime { get; set; }

        /// <summary>
        /// 包含DateTime 和Body
        /// </summary>
        private DataNotification dataNotification;
        /// <summary>
        /// 对Dlms的DataNotification body 结构 自定义解析为该类型
        /// </summary>
        public CustomAlarm CustomAlarm { get; set; } = new CustomAlarm();
        /// <summary>
        /// 根据pushid AlarmDescriptor1 解析之后的上报具体类型
        /// </summary>
        public AlarmType AlarmType { get; set; }


    }
    public class DataNotificationViewModel : BindableBase
    {
        public DataNotificationViewModel()
        {
            DataNotifications = new ObservableCollection<DataNotificationModel>();
        }
        public ObservableCollection<DataNotificationModel> DataNotifications { get; set; }
    }
}
