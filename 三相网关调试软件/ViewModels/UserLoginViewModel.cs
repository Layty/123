using System;
using System.Data.OleDb;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RestSharp;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.Model;
using 三相智慧能源网关调试软件.Properties;
using 三相智慧能源网关调试软件.UserLoginServiceReference;
using 三相智慧能源网关调试软件.Views;

namespace 三相智慧能源网关调试软件.ViewModels
{
    public interface IUserRepository
    {
        UserLoginModel LoginModel { get; set; }
        void Login();
        void Exit();
    }

    /// <summary>
    /// 通过webApi进行登录
    /// </summary>
    public class UserWebApi : ObservableObject, IUserRepository
    {
        public UserLoginModel LoginModel { get; set; } = new UserLoginModel();


        public async void Login()
        {
            try
            {
                LoginModel.IsLoginStatus = false;
                if (string.IsNullOrWhiteSpace(LoginModel.UserName) || string.IsNullOrWhiteSpace(LoginModel.Password))
                {
                    new MessageBoxWindow() { Message = "请输入用户名和密码！" }.ShowDialog();
                    return;
                }

                LoginModel.Report = "正在登录...";
                var resultResponse = await Task.Run(() =>
                {
                    var client =
                        new RestClient(
                            $"{Settings.Default.WebApiUrl}/UserLogin?userName={LoginModel.UserName}&password={LoginModel.Password}");
                    var request = new RestRequest(Method.POST) { Timeout = 2000 };

                    IRestResponse response = client.Execute(request);
                    return response;
                });

                if (resultResponse.IsSuccessful)
                {
                    LoginModel.SucceedLoginTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
                    LoginModel.LoginResult = true;
                    StrongReferenceMessenger.Default.Send("ni hao ya da shuai bi", "Snackbar");
                    LoginModel.Report = "登录成功";
                }
                else
                {
                    if (resultResponse.ErrorMessage == null)
                    {
                        LoginModel.Report = "用户名或密码错误";
                    }
                    else
                    {
                        LoginModel.Report = resultResponse.ErrorMessage;
                    }

                    LoginModel.LoginResult = false;
                }
            }
            catch (Exception ex)
            {
                LoginModel.Report = ex.Message;
            }
            finally
            {
                this.LoginModel.IsLoginStatus = true;
            }
        }

        public void Exit()
        {

        }
    }
    public class UserBackDoor : IUserRepository
    {
        public UserLoginModel LoginModel { get; set; } = new UserLoginModel();

        public void Login()
        {
            LoginModel.SucceedLoginTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
            LoginModel.LoginResult = true;
            StrongReferenceMessenger.Default.Send("后门登录", "Snackbar");
            LoginModel.Report = "登录成功";
        }

        public void Exit()
        {

        }
    }

    public class UseWpf : IUserRepository
    {
        public UserLoginModel LoginModel { get; set; } = new UserLoginModel();
        public async void Login()
        {
            using (UserLoginClient loginClient = new UserLoginClient())
            {
                try
                {
                    var b = await loginClient.LoginAsync(LoginModel.UserName, LoginModel.Password);
                    if (b)
                    {
                        LoginModel.SucceedLoginTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
                        LoginModel.LoginResult = true;
                        //                        Messenger.Default.Send(true, "LoginResult");
                        LoginModel.Report = "登录成功";
                    }
                    else
                    {
                        LoginModel.LoginResult = false;
                        //                        Messenger.Default.Send("用户名或密码错误！！！");
                        LoginModel.Report = "用户名或密码错误";
                    }
                }
                catch (Exception e)
                {
                    LoginModel.Report = e.Message;
                }
            }
        }

        public void Exit()
        {

        }
    }

    public class UseAccess : IUserRepository
    {
        public UserLoginModel LoginModel { get; set; } = new UserLoginModel();

        public void Exit()
        {

        }
        private readonly string _connectionStr = Settings.Default.AccessConnectionStr +
                                                 "Jet OLEDB:Database Password = 5841320;User Id=Admin;";

        public void Login()
        {
            LoginModel.Report = "";
            int result;
            using (OleDbConnection dbConnection = new OleDbConnection(_connectionStr))
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
                //SaveUserInfoToResource();
            }
            else
            {
                LoginModel.LoginResult = false;

                LoginModel.Report = "用户名或密码错误";
            }
        }
    }
    public class UserLoginViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;

        public UserLoginViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            LoginModel = new UserLoginModel();
            LoginModel.LoginErrorCounts = 0;
            ReadUserInfoFromResource();

            LoginCommand = new RelayCommand( () =>
            {
                _userRepository.Login();
                LoginModel.LoginResult = _userRepository.LoginModel.LoginResult;
            });
            ExitApplicationCommand = new RelayCommand(ApplicationShutdown);
            SaveUserInfoToResourceCommand = new RelayCommand(SaveUserInfoToResource);
        }

        public RelayCommand SaveUserInfoToResourceCommand { get; set; }

        public UserLoginModel LoginModel { get; set; }

        public RelayCommand LoginCommand { get; set; }

        public RelayCommand ExitApplicationCommand { get; set; }

        /// <summary>
        /// 关闭系统
        /// </summary>
        public void ApplicationShutdown()
        {

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
                Settings.Default.CurrentPassword = CEncoder.Encode(LoginModel.Password);
            }

            Settings.Default.CurrentUser = LoginModel.UserName;
            Settings.Default.PasswordSave = LoginModel.KeepPassword;
            Settings.Default.Save();
        }

    }
}