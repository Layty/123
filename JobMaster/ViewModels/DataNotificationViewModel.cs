using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.DataNotification;
using MyDlmsStandard.Axdr;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace JobMaster.ViewModels
{
    public class CustomAlarm
    {
        private DlmsStructure structure = new DlmsStructure();

        public AxdrOctetStringFixed PushId { get; set; }
        public AxdrOctetString CosemLogicalDeviceName { get; set; }
        public AxdrIntegerUnsigned32 AlarmDescriptor1 { get; set; }
        public AxdrIntegerUnsigned32 AlarmDescriptor2 { get; set; }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (structure.PduStringInHexConstructor(ref pduStringInHex))
            {
                PushId = new AxdrOctetStringFixed(6);
                var pid = structure.Items[0].Value.ToString();
                if (!PushId.PduStringInHexConstructor(ref pid)) return false;
                var deviceName = structure.Items[1].ToPduStringInHex().Substring(2);
                CosemLogicalDeviceName = new AxdrOctetString();
                if (!CosemLogicalDeviceName.PduStringInHexConstructor(ref deviceName)) return false;
                var descriptor1 = structure.Items[2].Value.ToString();
                AlarmDescriptor1 = new AxdrIntegerUnsigned32();
                if (!AlarmDescriptor1.PduStringInHexConstructor(ref descriptor1)) return false;
                var descriptor2 = structure.Items[3].Value.ToString();
                AlarmDescriptor2 = new AxdrIntegerUnsigned32();
                if (!AlarmDescriptor2.PduStringInHexConstructor(ref descriptor2)) return false;

                return true;
            }


            return false;
        }
    }
    [Flags]
    public enum AlarmRegisterObject2
    {
        [JsonProperty("停电")] PowerOff = 0x00000001,
        [JsonProperty("复电")] PowerOn = 0x00000004,
        [JsonProperty("过载")] OverLoad = 0x10000000,
        [JsonProperty("过流")] OverCurrent = 0x08000000,
        [JsonProperty("漏电流")] ByPass = 0x02000000,
    }
    public enum AlarmType
    {
        Unknown,
        [JsonProperty("停电")] PowerOff,
        [JsonProperty("复电")] PowerOn,
        [JsonProperty("漏电流")] ByPass,
        烟感and水浸变位,
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

        ///// <summary>
        ///// 根据pushid AlarmDescriptor1 解析之后的上报具体类型
        ///// </summary>
        //[JsonConverter(typeof(StringEnumConverter))]
        public string AlarmType { get; set; }
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