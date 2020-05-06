using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.Model
{
    public class UserLoginModel : ObservableObject, IDataErrorInfo
    {
      

        [Required(ErrorMessage = "用户名不能为空！")]
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }

        private string _userName;
        [Required(ErrorMessage = "密码不能为空！")]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        private bool _loginResult;

        public bool LoginResult
        {
            get => _loginResult;
            set
            {
                _loginResult = value;
                RaisePropertyChanged();
            }
        }

        private bool _isCancel;

        public bool IsCancel
        {
            get => _isCancel;
            set
            {
                _isCancel = value;
                RaisePropertyChanged();
            }
        }

        private string _succeedLoginTime;

        public string SucceedLoginTime
        {
            get { return _succeedLoginTime; }
            set { _succeedLoginTime = value; RaisePropertyChanged(); }
        }

        private byte _loginErrorCounts;

        public byte LoginErrorCounts
        {
            get { return _loginErrorCounts; }
            set { _loginErrorCounts = value; RaisePropertyChanged(); }
        }


        public string this[string columnName]
        {
            get
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = columnName;
                var res = new List<ValidationResult>();
                var result = Validator.TryValidateProperty(this.GetType().GetProperty(columnName).GetValue(this, null), vc, res);
                if (res.Count > 0)
                {
                    AddDic(dataErrors, vc.MemberName);
                    return string.Join(Environment.NewLine, res.Select(r => r.ErrorMessage).ToArray());
                }
                RemoveDic(dataErrors, vc.MemberName);
                return null;
            }
        }

    
        /// <summary>
        /// 表当验证错误集合
        /// </summary>
        private Dictionary<String, String> dataErrors = new Dictionary<String, String>();

        public string Error { get; }
        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private void RemoveDic(Dictionary<string, string> dics, string dicKey)
        {
            dics.Remove(dicKey);
        }

        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private void AddDic(Dictionary<string, string> dics, string dicKey)
        {
            if (!dics.ContainsKey(dicKey)) dics.Add(dicKey, "");
        }
    }
}