using System;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public class CaptureObjectDefinition : IToDlmsDataItem, IPduStringInHexConstructor
    {
        public ushort ClassId { get; set; }
        public string LogicalName { get; set; }
        public sbyte AttributeIndex { get; set; }
        public ushort DataIndex { get; set; }

        public DLMSDataItem ToDlmsDataItem()
        {
            DlmsStructure dlmsStructure = new DlmsStructure();
            dlmsStructure.Items = new DLMSDataItem[4];
            dlmsStructure.Items[0] = new DLMSDataItem(DataType.UInt16, ClassId.ToString("X4"));
            dlmsStructure.Items[1] = new DLMSDataItem(DataType.OctetString, MyConvert.ObisToHexCode(LogicalName));
            dlmsStructure.Items[2] = new DLMSDataItem(DataType.Int8, AttributeIndex.ToString("X2"));
            dlmsStructure.Items[3] = new DLMSDataItem(DataType.UInt16, DataIndex.ToString("X4"));
            return new DLMSDataItem(DataType.Structure, dlmsStructure.ToPduBytes());
        }

        public static CaptureObjectDefinition CreateFromDlmsData(DLMSDataItem ddi)
        {
            if (ddi == null || !(ddi.DataType is DataType.Structure))
            {
                return null;
            }

            DlmsStructure dlmsStructure = new DlmsStructure();
            var structrue = ddi.ToPduStringInHex();
            structrue = structrue.Substring(2);
            dlmsStructure.PduStringInHexConstructor(ref structrue);
            if (dlmsStructure.Items.Length != 4)
            {
                return null;
            }

            CaptureObjectDefinition captureObjectDefinition = new CaptureObjectDefinition();
            if (dlmsStructure.Items[0].DataType != DataType.UInt16)
            {
                return null;
            }

            captureObjectDefinition.ClassId = Convert.ToUInt16(dlmsStructure.Items[0].ValueBytes.ByteToString(), 16);
            if (dlmsStructure.Items[1].DataType != DataType.OctetString)
            {
                return null;
            }

            captureObjectDefinition.LogicalName =
                MyConvert.GetObisOriginal(dlmsStructure.Items[1].ValueBytes.ByteToString().Substring(2));

            if (string.IsNullOrEmpty(captureObjectDefinition.LogicalName))
            {
                return null;
            }

            if (dlmsStructure.Items[2].DataType != DataType.Int8)
            {
                return null;
            }

            captureObjectDefinition.AttributeIndex =
                Convert.ToSByte(dlmsStructure.Items[2].ValueBytes.ByteToString(), 16);
            if (dlmsStructure.Items[3].DataType != DataType.UInt16)
            {
                return null;
            }

            captureObjectDefinition.DataIndex = Convert.ToUInt16(dlmsStructure.Items[3].ValueBytes.ByteToString(), 16);
            return captureObjectDefinition;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            throw new NotImplementedException();
        }
    }
}