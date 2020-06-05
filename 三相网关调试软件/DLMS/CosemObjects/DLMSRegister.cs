using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/06/04 15:36:53
        主要用途：
        更改记录：
    */
    public class DLMSRegister : DLMSObject, IDLMSBase,INotifyPropertyChanged
    {
      
        public object Value
        {
            get => _Value;
            set { _Value = value;
                OnPropertyChanged();
            }
        }
        private object _Value;

        public double Scalar { get; set; } = 1.0;
        public Unit Unit { get; set; }

        public DLMSRegister(string logicName)
        {
            LogicalName = logicName;
            this.ObjectType = ObjectType.Register;
            Version = 0;
        }

        public byte[] GetLogicName() => GetAttributeData(1);
        public byte[] GetValue() => GetAttributeData(2);
        public byte[] GetScalar_Unit() => GetAttributeData(3);

     
        public string[] GetNames()
        {
            return new[] {LogicalName, "Value", "Scalar_Unit"};
        }

        public int GetAttributeCount()
        {
            return 3;
        }

        public int GetMethodCount()
        {
            return 1;
        }

        public DataType GetDataType(int index)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}