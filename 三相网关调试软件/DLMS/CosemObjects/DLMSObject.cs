﻿using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    /// <summary>
    /// 接口类base本身没有明确规定，它只包含一个属性“逻辑名”
    /// </summary>
    public abstract class DLMSObject
    {
        public MyDLMSSettings Settings { get; set; }
        public string Description { get; set; }

        public  string LogicalName { get; set; }

        public  ObjectType ObjectType { get; set; }
        public ushort ShortName { get; set; }

        public object Tag { get; set; }
        public  byte Version { get; set; } = 0;

        public static void ValidateLogicalName(string ln)
        {
            if (ln.Split('.').Length != 6)
            {
                throw new Exception("Invalid Logical Name.");
            }
        }

        public byte[] GetAttributeData(byte attrId)
        {
            GetRequest getRequest = new GetRequest(new CosemAttributeDescriptor(this, attrId));
            return getRequest.ToPduBytes();
        }


        public byte[] SetAttributeData(byte attrId, DLMSDataItem dlmsDataItem)
        {
            SetRequest setRequest = new SetRequest(new CosemAttributeDescriptor(this, attrId), dlmsDataItem);
            return setRequest.ToPduBytes();
        }

        public byte[] ActionExecute(byte methodIndex, DLMSDataItem dlmsDataItem)
        {
            ActionRequest actionRequest = new ActionRequest(new CosemMethodDescriptor(this, methodIndex), dlmsDataItem);
            return actionRequest.ToPduBytes();
        }

        public CosemMethodDescriptor GetCosemMethodDescriptor(byte methodIndex)
        {
            return new CosemMethodDescriptor(this, methodIndex);
        }

        public CosemAttributeDescriptor GetCosemAttributeDescriptor(byte attributeIndex)
        {
            return new CosemAttributeDescriptor(this, attributeIndex);
        }
    }
}