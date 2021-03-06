
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
   
    public class ScriptAction
    {
        public CosemObject Target { get; set; }
        public ScriptActionType ScriptActionType { get; set; }
        public ObjectType ObjectType { get; set; }
        public string LogicalName { get; set; }
        public DataType ParameterDataType { get; set; }
        public int Index { get; set; }
        public object Parameter { get; set; }
    }
}