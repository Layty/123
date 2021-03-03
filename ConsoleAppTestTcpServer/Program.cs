using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.Model;

namespace ConsoleAppTestTcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                for (int i = 1; i <= 50; i++)
                {
                    var i1 = i;

                    HeartBeatFrame heartBeatFrame = new HeartBeatFrame();

                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Connect("192.168.1.155", 8881);
                    heartBeatFrame.MeterAddressBytes = Encoding.Default.GetBytes(i1.ToString().PadLeft(12, '0'));
                    Thread.Sleep(500);
                    tcpClient.Client.Send(heartBeatFrame.ToPduStringInHex().StringToByte());
                    Console.WriteLine(i1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.ReadLine();
        }
    }
}