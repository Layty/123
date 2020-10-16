using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ConsoleWcfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            StartLogin();
        }


        static void StartLogin()
        {
            Uri baseUri = new Uri("http://localhost:9001/");

            using (ServiceHost host = new ServiceHost(typeof(UserLogin), baseUri))
            {
                host.AddServiceEndpoint(typeof(ILogin), new WSHttpBinding(), "Login");

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);
                host.Open();
                Console.WriteLine("Login服务已启动");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}