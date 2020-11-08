using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public class ProfileGenericEntryDescriptor : IToDlmsDataItem
    {
        public uint FromIndex { get; set; }
        public uint ToIndex { get; set; }
        public ushort FromSelectedValue { get; set; }
        public ushort ToSelectedValue { get; set; }

        public DlmsDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DlmsDataItem[4];
            dlmsStructure.Items[0] = new DlmsDataItem(DataType.UInt32, FromIndex.ToString("X8"));
            dlmsStructure.Items[1] = new DlmsDataItem(DataType.UInt32, ToIndex.ToString("X8"));
            dlmsStructure.Items[2] = new DlmsDataItem(DataType.UInt16, FromSelectedValue.ToString("X4"));
            dlmsStructure.Items[3] = new DlmsDataItem(DataType.UInt16, ToSelectedValue.ToString("X4"));
            DlmsDataItem dlmsDataItem = new DlmsDataItem(DataType.Structure,dlmsStructure);

            return dlmsDataItem;
        }
    }
}