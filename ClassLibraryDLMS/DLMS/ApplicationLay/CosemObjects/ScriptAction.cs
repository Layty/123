
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.CosemObjects
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