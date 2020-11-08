using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
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