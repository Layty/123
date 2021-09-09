using System;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;


namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemRegisterModel : CosemRegister
    {
        public ErrorCode LastResult
        {
            get => _lastResult;
            set
            {
                _lastResult = value;
                OnPropertyChanged();
            }
        }

        private ErrorCode _lastResult;
        public string RegisterName { get; set; }

        public string CompleteData
        {
            get => _completeData;
            set
            {
                _completeData = value;
                OnPropertyChanged();
            }
        }

        private string _completeData;

        public string ParserData()
        {
            if (ScalarUnit == null)
            {
                return "";
            }

            if (Value == null)
            {
                return "";
            }

            var index = Convert.ToSByte(ScalarUnit.Scalar.Value.ToString(), 16);

            double point = 1;
            switch (index)
            {
                case -1:
                    point = 0.1;
                    break;
                case -2:
                    point = 0.01;
                    break;
                case -3:
                    point = 0.001;
                    break;
                case -4:
                    point = 0.0001;
                    break;
                case 1:
                    point = 10;
                    break;

                case 2:
                    point = 100;
                    break;
                case 3:
                    point = 1000;
                    break;
                case 4:
                    point = 10000;
                    break;
                default:
                    break;
                    ;
            }

            if (Value.DataType == DataType.Array)
            {
                //修复当网关温湿度为array类时,可以解析格式化输出
                var arry = (DLMSArray) Value.Value;
                foreach (var item in arry.Items)
                {
                    CompleteData += (Convert.ToUInt32(item.Value.ToString(), 16) * point) + ScalarUnit.UnitDisplay+"\r\n";
                }
            }
            else
            {
                var value = Convert.ToUInt32(Value.Value.ToString(), 16);
                CompleteData = (value * point) + ScalarUnit.UnitDisplay;
            }

            return CompleteData;
        }


        public CustomCosemRegisterModel(string logicName) : base(logicName)
        {
        }
    }
}