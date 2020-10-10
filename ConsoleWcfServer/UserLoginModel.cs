using System;
using System.Data.OleDb;
using ConsoleWcfServer.Properties;

namespace ConsoleWcfServer
{
    public class UserLoginModel:ILogin
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool KeepPassword { get; set; }

        /// <summary>
        /// 进度报告
        /// </summary>
        public string Report { get; set; }

        public bool LoginResult { get; set; }

      

        public string SucceedLoginTime { get; set; }

        public byte LoginErrorCounts { get; set; }


        private string ConnectionStr = Settings.Default.AccessConnectionStr +
                                       "Jet OLEDB:Database Password = 5841320;User Id=Admin;";

        public bool Login(string UserName,string Password)
        {
            Report = "";
            int result;
            using (OleDbConnection dbConnection = new OleDbConnection(ConnectionStr))
            {
                dbConnection.Open();

                string sqlSel = "select count(*) from UserInfo where userName = '" + UserName +
                                "' and password = '" + Password + "'";
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
                
                //SaveUserInfoToResource();
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