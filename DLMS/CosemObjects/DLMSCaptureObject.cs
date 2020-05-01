namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class DLMSCaptureObject
    {
        public int AttributeIndex { get; set; }

        public int DataIndex { get; set; }

        public DLMSCaptureObject()
        {
        }

        public DLMSCaptureObject(int attributeIndex, int dataIndex)
        {
            AttributeIndex = attributeIndex;
            DataIndex = dataIndex;
        }
    }
}