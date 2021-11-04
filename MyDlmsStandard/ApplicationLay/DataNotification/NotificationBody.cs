namespace MyDlmsStandard.ApplicationLay.DataNotification
{
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
    public class NotificationBodyV1
    {
        public DlmsDataItem DataValue { get; set; }

        public NotificationBodyV1(DlmsDataItem dlmsDataItem)
        {
            DataValue = dlmsDataItem;
        }
        public static NotificationBodyV1 Parse(ref string pduStringInHex)
        {
            var dataValue = new DlmsDataItem();
            if (!dataValue.PduStringInHexConstructor(ref pduStringInHex))
            {
                return null;
            }

            return new NotificationBodyV1(dataValue);
        }
    }
}