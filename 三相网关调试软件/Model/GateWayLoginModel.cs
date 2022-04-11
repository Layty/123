using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 三相智慧能源网关调试软件.Model
{
    public class GateWayLoginModel : ObservableValidator
    {
        [Required(ErrorMessage = "网关IP不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged();
            }
        }

        private string _ipAddress;

        [Required(ErrorMessage = "端口号不能为空！")]
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        private int _port;
    }
}