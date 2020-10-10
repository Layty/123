using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.DataNotification
{
    public class DataNotification : IPduStringInHexConstructor
    {
        public Command Command { get; set; } = Command.DataNotification;

        public AxdrUnsigned32 LongInvokeIdAndPriority { get; set; }

        public AxdrOctetStringFixed DateTime { get; set; }
        public DLMSDataItem NotificationBody { get; set; }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            var NotificationTag = pduStringInHex.Substring(0, 2);
            if (NotificationTag != "0F")
            {
                return false;
            }

            pduStringInHex = pduStringInHex.Substring(2);
            LongInvokeIdAndPriority = new AxdrUnsigned32();
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

            NotificationBody = new DLMSDataItem();
            if (!NotificationBody.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}