using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    public class ProfileGenericEntryDescriptor : IToDlmsDataItem
    {
        public uint FromIndex { get; set; }

        /// <summary>
        /// to_entry==0:表明可 能达到的最大条目,
        /// </summary>
        public uint ToIndex { get; set; }

        public ushort FromSelectedValue { get; set; }

        /// <summary>
        ///  ToSelectedValue==0 表明可能达到的最大索引号。
        /// </summary>
        public ushort ToSelectedValue { get; set; }

        public DlmsDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DlmsDataItem[4];
            dlmsStructure.Items[0] = new DlmsDataItem(DataType.UInt32, FromIndex.ToString("X8"));
            dlmsStructure.Items[1] = new DlmsDataItem(DataType.UInt32, ToIndex.ToString("X8"));
            dlmsStructure.Items[2] = new DlmsDataItem(DataType.UInt16, FromSelectedValue.ToString("X4"));
            dlmsStructure.Items[3] = new DlmsDataItem(DataType.UInt16, ToSelectedValue.ToString("X4"));
            DlmsDataItem dlmsDataItem = new DlmsDataItem(DataType.Structure, dlmsStructure);

            return dlmsDataItem;
        }
    }
}