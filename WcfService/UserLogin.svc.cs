using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfService.Properties;

namespace WcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“UserLogin”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 UserLogin.svc 或 UserLogin.svc.cs，然后开始调试。
    public class UserLogin : IUserLogin
    {


        /// <summary>
        /// 进度报告
        /// </summary>
        public string Report { get; set; }

        public bool LoginResult { get; set; }

        public string SucceedLoginTime { get; set; }

        private string ConnectionStr = Settings.Default.AccessConnectionStr +
                                       "Jet OLEDB:Database Password = 5841320;User Id=Admin;";

        public bool Login(string userName, string password)
        {
            Report = "";
            int result;
            using (OleDbConnection dbConnection = new OleDbConnection(ConnectionStr))
            {
                dbConnection.Open();

                string sqlSel = "select count(*) from UserInfo where userName = '" + userName +
                                "' and password = '" + password + "'";
                using (OleDbCommand cmd = new OleDbCommand(sqlSel, dbConnection))
                {
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
          

            if (result > 0)
            {
                SucceedLoginTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
                LoginResult = true;
                Report = "登录成功";
                return true;
            }
            else
            {
                LoginResult = false;
                Report = "用户名或密码错误";
                return false;
            }
        }
    }
}
