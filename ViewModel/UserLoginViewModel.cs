using System;
using System.Data.OleDb;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.Model;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class UserLoginViewModel : ViewModelBase
    { 
        public UserLoginViewModel()
        {
            if (IsInDesignModeStatic)
            {
                LoginModel = new UserLoginModel {UserName = "Test", Password = "123456", KeepPassword = true};
            }
            else
            {
                LoginModel = new UserLoginModel();
                ReadUserInfoFromResource();
               
                LoginCommand = new RelayCommand(Login);

                ExitApplicationCommand = new RelayCommand(ApplicationShutdown);
                
                SaveUserInfoToResourceCommand = new RelayCommand(SaveUserInfoToResource);
            }
        }

        public RelayCommand SaveUserInfoToResourceCommand
        {
            get => _saveUserInfoToResourceCommand;
            set
            {
                _saveUserInfoToResourceCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _saveUserInfoToResourceCommand;


        private UserLoginModel _loginModel;

        public UserLoginModel LoginModel
        {
            get => _loginModel;
            set
            {
                _loginModel = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _loginCommand;

        public RelayCommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(Login);
                }

                return _loginCommand;
            }
            set
            {
                _loginCommand = value;
                RaisePropertyChanged();
            }
        }


        private RelayCommand _exitApplicationCommand;

        public RelayCommand ExitApplicationCommand
        {
            get => _exitApplicationCommand;
            set
            {
                _exitApplicationCommand = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        public void ApplicationShutdown()
        {
            Messenger.Default.Send("", "ApplicationShutdown");
        }

        /// <summary>
        /// 读取本地配置信息
        /// </summary>
        public void ReadUserInfoFromResource()
        {
            Settings.Default.Reload();
            LoginModel.KeepPassword = Settings.Default.PasswordSave;
            if (LoginModel.KeepPassword)
            {
                LoginModel.UserName = Settings.Default.CurrentUser;
                //   LoginModel.Password = Properties.Settings.Default.CurrentPassword;
                LoginModel.Password = CEncoder.Decode(Settings.Default.CurrentPassword);
                if (LoginModel.UserName == "" || LoginModel.Password == "")
                {
                    LoginModel.KeepPassword = false;
                }
            }
        }

        /// <summary>
        /// 保存登录信息本地配置表
        /// </summary>
        public void SaveUserInfoToResource()
        {
            if (LoginModel.KeepPassword)
            {
//                Properties.Settings.Default.CurrentPassword = LoginModel.Password;
                Settings.Default.CurrentPassword = CEncoder.Encode(LoginModel.Password);
            }

           //var sw=new SwatchesProvider().Swatches;
           //sw.FirstOrDefault(t => t.Name.Equals(""));


           //new PaletteHelper();
            Settings.Default.CurrentUser = LoginModel.UserName;
            Settings.Default.PasswordSave = LoginModel.KeepPassword;

            Settings.Default.Save();
        }

        private string ConnectionStr = Settings.Default.AccessConnectionStr +
                                       "Jet OLEDB:Database Password = 5841320;User Id=Admin;";

        public void Login()
        {
            LoginModel.Report = "";
            int result;
            using (OleDbConnection dbConnection = new OleDbConnection(ConnectionStr))
            {
                dbConnection.Open();

                string sqlSel = "select count(*) from UserInfo where userName = '" + LoginModel.UserName +
                                "' and password = '" + LoginModel.Password + "'";
                using (OleDbCommand cmd = new OleDbCommand(sqlSel, dbConnection))
                {
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            if (result > 0)
            {
                LoginModel.SucceedLoginTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
                LoginModel.LoginResult = true;
                LoginModel.Report = "登录成功";
            }
            else
            {
                LoginModel.LoginResult = false;
                Messenger.Default.Send("用户名或密码错误！！！");
                LoginModel.Report = "用户名或密码错误";
            }
        }
    }
}