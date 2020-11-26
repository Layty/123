using System.Collections.Generic;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsNetCore.ApplicationLay.CosemObjects.DataStorage
{
    public class ProfileGenericRangeDescriptor : IToDlmsDataItem
    {
        public CaptureObjectDefinition RestrictingObject { get; set; }
        public DlmsDataItem FromValue { get; set; }
        public DlmsDataItem ToValue { get; set; }
        public List<CaptureObjectDefinition> SelectedValues { get; set; }

        public DlmsDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DlmsDataItem[4];
            dlmsStructure.Items[0] = RestrictingObject.ToDlmsDataItem();
            dlmsStructure.Items[1] = FromValue;
            dlmsStructure.Items[2] = ToValue;
            DLMSArray dlmsArray = new DLMSArray();
            if (SelectedValues == null || SelectedValues.Count == 0)
            {
                dlmsArray.Items = new DlmsDataItem[0];
            }
            else
            {
                dlmsArray.Items = new DlmsDataItem[SelectedValues.Count];
                for (int i = 0; i < SelectedValues.Count; i++)
                {
                    dlmsArray.Items[i] = SelectedValues[i].ToDlmsDataItem();
                }
            }

            dlmsStructure.Items[3] = new DlmsDataItem(DataType.Array, dlmsArray.ToPduStringInHex());

            DlmsDataItem dlmsDataItem = new DlmsDataItem(DataType.Structure) {Value = dlmsStructure};
            return dlmsDataItem;
        }
    }
}