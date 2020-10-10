using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2.ServiceReference1;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceReference1.LoginClient Client=new ServiceReference1.LoginClient();
            Console.WriteLine(Client.Login("admin","123456"));
            Console.ReadLine();
            


        }
    }
}
