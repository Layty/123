using System;
using System.Collections.Generic;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public sealed class ScriptTable : CosemObject, IDlmsBase
    {
        public List<Script> Scripts { get; set; }
 
        public ScriptTable()
        {
            Scripts = new List<Script>();
            LogicalName = "0.0.10.0.0.255";
            Version = 1;
        
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.ScriptTable);
        }

        public ScriptTable(string logicalName, ObjectType objectType)
        {
        }

        public CosemAttributeDescriptor GetLogicalAttributeDescriptor() => GetCosemAttributeDescriptor(1);

        public CosemAttributeDescriptor GetScriptsAttributeDescriptor() => GetCosemAttributeDescriptor(2);

        public CosemMethodDescriptor GetScriptExecuteCosemMethodDescriptor() => GetCosemMethodDescriptor(1);


        string[] IDlmsBase.GetNames()
        {
            return new string[2]
            {
                LogicalName,
                "Scripts"
            };
        }

        int IDlmsBase.AttributeCount => 2;

        int IDlmsBase.MethodCount => 1;

        DataType IDlmsBase.GetDataType(int index)
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