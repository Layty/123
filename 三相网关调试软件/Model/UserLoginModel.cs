using System.ComponentModel.DataAnnotations;

namespace 三相智慧能源网关调试软件.Model
{

    public class UserLoginModel : ValidateModelBase
    {
        [Required(ErrorMessage = "The user name cannot be empty！")]
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _userName;

        [Required(ErrorMessage = "The password cannot be empty！")]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _password;

        private bool _keepPassword;

        public bool KeepPassword
        {
            get => _keepPassword;
            set
            {
                _keepPassword = value;
                OnPropertyChanged();
            }
        }

        private string _report;

        /// <summary>
        /// 进度报告
        /// </summary>
        public string Report
        {
            get => _report;
            set
            {
                _report = value;
                OnPropertyChanged();
            }
        }

        private bool _loginResult;

        public bool LoginResult
        {
            get => _loginResult;
            set
            {
                _loginResult = value;
                OnPropertyChanged();
            }
        }

        private bool _isCancel;

        public bool IsCancel
        {
            get => _isCancel;
            set
            {
                _isCancel = value;
                OnPropertyChanged();
            }
        }

        private string _succeedLoginTime;

        public string SucceedLoginTime
        {
            get => _succeedLoginTime;
            set
            {
                _succeedLoginTime = value;
                OnPropertyChanged();
            }
        }

        private byte _loginErrorCounts;

        public byte LoginErrorCounts
        {
            get => _loginErrorCounts;
            set
            {
                _loginErrorCounts = value;
                OnPropertyChanged();
            }
        }

    }
}