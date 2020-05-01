using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.HDLC;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public sealed class ScriptTable : DLMSObject, IDLMSBase
    {
        public override string LogicalName { get; set; } = "0.0.10.0.0.255";
        public override byte Version { get; set; } = 1;
        public override ObjectType ObjectType { get; set; } = ObjectType.ScriptTable; //9
        public List<Script> Scripts { get; set; }

        public ScriptTable()
        {
            Scripts = new List<Script>();
        }

        public void GetScripts()
        {
            GetAttributeData(2);
        }

        public void ScriptExecute(ushort scriptId)
        {
            DLMSDataItem dlmsData =
                new DLMSDataItem(DataType.UInt16, BitConverter.GetBytes(scriptId).Reverse().ToArray());
            ActionExecute(1, dlmsData);
        }

        string[] IDLMSBase.GetNames()
        {
            return new string[2]
            {
                LogicalName,
                "Scripts"
            };
        }

        int IDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IDLMSBase.GetMethodCount()
        {
            return 1;
        }

        public DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Array;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }
    }
}