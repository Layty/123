using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.ProfileGeneric
{
    public class ProfileGenericEntryDescriptor : IToDlmsDataItem
    {
        /// <summary>
        /// 要检索的第一个条目  注3:条目的计数和数值的选择从1开始。
        /// </summary>
        public uint FromEntry { get; set; } = 1;

        /// <summary>
        /// 要检索的最后一个条目。to_entry==0:表明可 能达到的最大条目,
        /// </summary>
        public uint ToEntry { get; set; }

        /// <summary>
        /// 要检索的第一个值的索引号
        /// </summary>
        public ushort FromSelectedValue { get; set; } = 1;

        /// <summary>
        ///  要检索的最后一个值的索引号,ToSelectedValue==0 表明可能达到的最大索引号。
        /// </summary>
        public ushort ToSelectedValue { get; set; }

        public DlmsDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DlmsDataItem[4];
            dlmsStructure.Items[0] = new DlmsDataItem(DataType.UInt32, FromEntry.ToString("X8"));
            dlmsStructure.Items[1] = new DlmsDataItem(DataType.UInt32, ToEntry.ToString("X8"));
            dlmsStructure.Items[2] = new DlmsDataItem(DataType.UInt16, FromSelectedValue.ToString("X4"));
            dlmsStructure.Items[3] = new DlmsDataItem(DataType.UInt16, ToSelectedValue.ToString("X4"));
            DlmsDataItem dlmsDataItem = new DlmsDataItem(DataType.Structure, dlmsStructure);

            return dlmsDataItem;
        }
    }
}