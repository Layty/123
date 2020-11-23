using System;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    public class CaptureObjectDefinition : IToDlmsDataItem, IPduStringInHexConstructor
    {
        public ushort ClassId { get; set; }
        public string LogicalName { get; set; }
        public sbyte AttributeIndex { get; set; }
        public ushort DataIndex { get; set; }

        public string Description { get;set; }

        public DlmsDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DlmsDataItem[4];
            dlmsStructure.Items[0] = new DlmsDataItem(DataType.UInt16, ClassId.ToString("X4"));
            dlmsStructure.Items[1] = new DlmsDataItem(DataType.OctetString, MyConvert.ObisToHexCode(LogicalName));
            dlmsStructure.Items[2] = new DlmsDataItem(DataType.Int8, AttributeIndex.ToString("X2"));
            dlmsStructure.Items[3] = new DlmsDataItem(DataType.UInt16, DataIndex.ToString("X4"));

            return new DlmsDataItem(DataType.Structure, dlmsStructure);
        }

        public static CaptureObjectDefinition CreateFromDlmsData(DlmsDataItem ddi)
        {
            if (ddi == null || !(ddi.DataType is DataType.Structure))
            {
                return null;
            }

            DlmsStructure dlmsStructure = new DlmsStructure();
            var structure = ddi.ToPduStringInHex();
           
            dlmsStructure.PduStringInHexConstructor(ref structure);
            if (dlmsStructure.Items.Length != 4)
            {
                return null;
            }

            CaptureObjectDefinition captureObjectDefinition = new CaptureObjectDefinition();
            if (dlmsStructure.Items[0].DataType != DataType.UInt16)
            {
                return null;
            }

            captureObjectDefinition.ClassId = Convert.ToUInt16(dlmsStructure.Items[0].Value.ToString(), 16);
            if (dlmsStructure.Items[1].DataType != DataType.OctetString)
            {
                return null;
            }

            captureObjectDefinition.LogicalName =
                MyConvert.GetObisOriginal(dlmsStructure.Items[1].Value.ToString());

            if (string.IsNullOrEmpty(captureObjectDefinition.LogicalName))
            {
                return null;
            }

            if (dlmsStructure.Items[2].DataType != DataType.Int8)
            {
                return null;
            }

            captureObjectDefinition.AttributeIndex =
                Convert.ToSByte(dlmsStructure.Items[2].Value.ToString(), 16);
            if (dlmsStructure.Items[3].DataType != DataType.UInt16)
            {
                return null;
            }

            captureObjectDefinition.DataIndex = Convert.ToUInt16(dlmsStructure.Items[3].Value.ToString(), 16);
            return captureObjectDefinition;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            throw new NotImplementedException();
        }
    }
}