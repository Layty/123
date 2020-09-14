using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Action;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    /// <summary>
    /// 接口类base本身没有明确规定，它只包含一个属性“逻辑名”
    /// </summary>
    public abstract class DLMSObject
    {
        public string LogicalName { get; set; }

        public string Description { get; set; }

        public ObjectType ObjectType { get; set; }
        public ushort ShortName { get; set; }

        public object Tag { get; set; }
        public byte Version { get; set; } = 0;

        public DLMSObject()
            : this(ObjectType.None, null, 0)
        {
        }

        protected DLMSObject(ObjectType objectType)
            : this(objectType, null, 0)
        {
        }

        protected DLMSObject(ObjectType objectType, string ln, ushort sn)
        {
            ObjectType = objectType;
            ShortName = sn;
            if (ln != null)
            {
                ValidateLogicalName(ln);
            }

            LogicalName = ln;
        }

        public static void ValidateLogicalName(string ln)
        {
            if (ln.Split('.').Length != 6)
            {
                throw new Exception("Invalid Logical Name.");
            }
        }


        public byte[] GetAttributeData(byte attrId)
        {
            GetRequest getRequest = new GetRequest();
            getRequest.GetRequestNormal = new GetRequestNormal(
                new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High),
                new AttributeDescriptor(this, attrId));
            return getRequest.ToPduBytes();
        }


        public byte[] SetAttributeData(byte attrId, DLMSDataItem dlmsDataItem)
        {
            SetRequestNormal setRequestNormal =
                new SetRequestNormal(new AttributeDescriptor(this, attrId), dlmsDataItem);
            return setRequestNormal.ToPduBytes();
        }


        public byte[] ActionExecute(byte methodIndex, DLMSDataItem dlmsDataItem)
        {
            ActionRequestNormal actionRequestNormal = new ActionRequestNormal(
                new CosemMethodDescriptor(this, methodIndex), dlmsDataItem
            );
            return actionRequestNormal.ToPduBytes();
        }

        public AttributeDescriptor GetCosemAttributeDescriptor(byte attributeIndex)
        {
            return new AttributeDescriptor(this, attributeIndex);
        }

        public CosemMethodDescriptor GetCosemMethodDescriptor(byte methodIndex)
        {
            return new CosemMethodDescriptor(this, methodIndex);
        }
    }
}