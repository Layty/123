using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public abstract class CosemObject : CosemBase, INotifyPropertyChanged
    {
        /// <summary>
        /// ClassId,分别指向对应的类1 对象类型 如类八为 时间，见 ObjectType定义
        /// </summary>
        public AxdrIntegerUnsigned16 ClassId
        {
            get => _classId;
            set
            {
                _classId = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned16 _classId;

        public string Description { get; set; }

        /// <summary>
        /// 短命
        /// </summary>
        public ushort ShortName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
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

        /// <summary>
        /// 静态方法，校验LogicalName，对字符串 通过 . 切割 进行简单校验
        /// </summary>
        /// <param name="ln"></param>
        public static void ValidateLogicalName(string ln)
        {
            if (ln.Split('.').Length != 6)
            {
                throw new Exception("Invalid Logical Name.");
            }
        }

        /// <summary>
        /// 获取Cosem对象某个属性的属性描述对象，输入为sbyte类型
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <returns></returns>
        public CosemAttributeDescriptor GetCosemAttributeDescriptor(sbyte attributeIndex)
        {
            return new CosemAttributeDescriptor(
                ClassId, new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                new AxdrInteger8(attributeIndex));
        }
        /// <summary>
        ///  获取Cosem对象某个属性的属性描述对象，输入为AxdrInteger8类型 ,推荐使用
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <returns></returns>
        public CosemAttributeDescriptor GetCosemAttributeDescriptor(AxdrInteger8 attributeIndex)
        {
            return new CosemAttributeDescriptor(
                ClassId, new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                attributeIndex);
        }
        /// <summary>
        /// 获取Cosem对象某个方法描述对象
        /// </summary>
        /// <param name="methodIndex"></param>
        /// <returns></returns>
        public CosemMethodDescriptor GetCosemMethodDescriptor(sbyte methodIndex)
        {
            return new CosemMethodDescriptor(
                ClassId,
                new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                new AxdrInteger8(methodIndex));
        }
        /// <summary>
        /// 获取Cosem对象某个方法描述对象
        /// </summary>
        /// <param name="methodIndex"></param>
        /// <returns></returns>
        public CosemMethodDescriptor GetCosemMethodDescriptor(AxdrInteger8 methodIndex)
        {
            return new CosemMethodDescriptor(
                ClassId,
                new AxdrOctetStringFixed(MyConvert.ObisToHexCode(LogicalName), 6),
                methodIndex);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 返回逻辑名和描述信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ShortName != 0)
            {
                return ShortName + " " + Description;
            }

            return LogicalName + " " + Description;
        }
    }
}