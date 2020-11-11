using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.DataNotification
{
    public class DataNotification : IPduStringInHexConstructor
    {
        public Command Command { get; set; } = Command.DataNotification;

        public AxdrIntegerUnsigned32 LongInvokeIdAndPriority { get; set; }

        public AxdrOctetStringFixed DateTime { get; set; }

        public NotificationBody NotificationBody { get; set; }
//        public DlmsDataItem NotificationBody { get; set; }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            var notificationTag = pduStringInHex.Substring(0, 2);
            if (notificationTag != "0F")
            {
                return false;
            }

            pduStringInHex = pduStringInHex.Substring(2);
            LongInvokeIdAndPriority = new AxdrIntegerUnsigned32();
            if (!LongInvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            DateTime = new AxdrOctetStringFixed(12);
            pduStringInHex = pduStringInHex.Substring(2);
            if (!DateTime.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            NotificationBody = new NotificationBody();
            if (!NotificationBody.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }

    public class NotificationBody : IPduStringInHexConstructor
    {
        public DlmsDataItem DataValue { get; set; }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            DataValue = new DlmsDataItem();
            if (!DataValue.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}