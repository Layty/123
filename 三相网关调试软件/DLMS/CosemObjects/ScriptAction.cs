using Gurux.DLMS.Objects.Enums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class ScriptAction
    {
        public DLMSObject Target { get; set; }
        public ScriptActionType ScriptActionType { get; set; }
        public ObjectType ObjectType { get; set; }
        public string LogicalName { get; set; }
        public DataType ParameterDataType { get; set; }
        public int Index { get; set; }
        public object Parameter { get; set; }
    }
}