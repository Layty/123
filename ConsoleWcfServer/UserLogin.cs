using System;
using System.Data.OleDb;
using ConsoleWcfServer.Properties;

namespace ConsoleWcfServer
{
    public class UserLogin:ILogin
    {
       
        /// <summary>
        /// 进度报告
        /// </summary>
        public string Report { get; set; }

        public bool LoginResult { get; set; }

        public string SucceedLoginTime { get; set; }

        private string ConnectionStr = Settings.Default.AccessConnectionStr +
                                       "Jet OLEDB:Database Password = 5841320;User Id=Admin;";

        public bool Login(string userName,string password)
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