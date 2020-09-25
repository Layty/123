using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public sealed class ScriptTable : CosemObject, IDLMSBase
    {

        public List<Script> Scripts { get; set; }

        public ScriptTable()
        {
            Scripts = new List<Script>();
            LogicalName = "0.0.10.0.0.255";
            Version = 1;
            ObjectType = ObjectType.ScriptTable;
        }

        public byte[] GetScripts() => GetAttributeData(2);

        public byte[] ScriptExecute(ushort scriptId)
        {
            DLMSDataItem dlmsData =
                new DLMSDataItem(DataType.UInt16, BitConverter.GetBytes(scriptId).Reverse().ToArray());
            return ActionExecute(1, dlmsData);
        }

        string[] IDLMSBase.GetNames()
        {
            return new string[2]
            {
                LogicalName,
                "Scripts"
            };
        }

        int IDLMSBase.GetAttributeCount() => 2;

        int IDLMSBase.GetMethodCount() => 1;

        DataType IDLMSBase.GetDataType(int index)
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