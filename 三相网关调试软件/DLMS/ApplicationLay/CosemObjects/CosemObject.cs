using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Action;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{  
    /// <summary>
    /// 接口类base本身没有明确规定，它只包含一个属性“逻辑名”
    /// </summary>
    public abstract class CosemBase
    {
        public string LogicalName { get; set; }
    }
  
    public abstract class CosemObject: CosemBase
    {
        public AxdrUnsigned16 ClassId { get; set; }
//        public ObjectType ObjectType { get; set; }
        public string Description { get; set; }

     
        public ushort ShortName { get; set; }

        public object Tag { get; set; }
        public byte Version { get; set; } = 0;

        protected CosemObject()
            : this(ObjectType.None, null, 0)
        {
        }

        protected CosemObject(ObjectType objectType)
            : this(objectType, null, 0)
        {
        }

        protected CosemObject(ObjectType objectType, string ln, ushort sn)
        {
            ClassId = MyConvert.GetClassIdByObjectType(objectType);
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


        public byte[] SetAttributeData(sbyte attrId, DLMSDataItem dlmsDataItem)
        {
            SetRequestNormal setRequestNormal =
                new SetRequestNormal(new CosemAttributeDescriptor(
                        ClassId, new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName) ,6),new AxdrInteger8(attrId)) 
                    , dlmsDataItem);
            return setRequestNormal.ToPduBytes();
        }


        public byte[] ActionExecute(sbyte methodIndex, DLMSDataItem dlmsDataItem)
        {
            ActionRequestNormal actionRequestNormal = new ActionRequestNormal(new CosemMethodDescriptor(
                    ClassId, 
                    new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                    new AxdrInteger8(methodIndex)), dlmsDataItem
            );
            return actionRequestNormal.ToPduBytes();
        }

        public CosemAttributeDescriptor GetCosemAttributeDescriptor(sbyte attributeIndex)
        {
            return new CosemAttributeDescriptor(
                ClassId, new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                new AxdrInteger8(attributeIndex));
        }
        public CosemAttributeDescriptor GetCosemAttributeDescriptor(AxdrInteger8 attributeIndex)
        {
            return new CosemAttributeDescriptor(
                ClassId, new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                attributeIndex);
        }
        public CosemMethodDescriptor GetCosemMethodDescriptor(sbyte methodIndex)
        {
            return  new CosemMethodDescriptor(
                ClassId,
                new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                new AxdrInteger8(methodIndex));
        }
    }
}