using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Axdr;
using ClassLibraryDLMS.DLMS.Common;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.CosemObjects
{
    /// <summary>
    /// 接口类base本身没有明确规定，它只包含一个属性“逻辑名”
    /// </summary>
    public abstract class CosemBase
    {
        public string LogicalName { get; set; }
    }

    public abstract class CosemObject : CosemBase, INotifyPropertyChanged
    {
        public AxdrUnsigned16 ClassId
        {
            get => _classId;
            set
            {
                _classId = value;
                OnPropertyChanged();
            }
        }

        private AxdrUnsigned16 _classId;


        public string Description { get; set; }

        public ushort ShortName { get; set; }

        public int Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        private int _version;


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


//        public byte[] SetAttributeData(sbyte attrId, DLMSDataItem dlmsDataItem)
//        {
//            SetRequestNormal setRequestNormal =
//                new SetRequestNormal(new AttributeDescriptor(
//                        ClassId, new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
//                        new AxdrInteger8(attrId))
//                    , dlmsDataItem);
//            return setRequestNormal.ToPduBytes();
//        }


//        public byte[] ActionExecute(sbyte methodIndex, DLMSDataItem dlmsDataItem)
//        {
//            ActionRequestNormal actionRequestNormal = new ActionRequestNormal(new CosemMethodDescriptor(
//                    ClassId,
//                    new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
//                    new AxdrInteger8(methodIndex)), dlmsDataItem
//            );
//            return actionRequestNormal.ToPduBytes();
//        }

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
            return new CosemMethodDescriptor(
                ClassId,
                new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                new AxdrInteger8(methodIndex));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            if (ShortName != 0)
            {
                return ShortName.ToString() + " " + Description;
            }

            return LogicalName + " " + Description;
        }
    }
}