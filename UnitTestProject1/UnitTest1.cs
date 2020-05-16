using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.Model.IIC;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IicInstantData vData = new IicInstantData();
            vData.ParseData(
                " 80 01 14 05 0E 0C 06 37 00 00 00 00 00 00 00 00 00 03 95 94 00 00 00 00 00 00 00 00 00 00 00 72 00 00 00 01 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 01 90 00 00 00 00 00 00 00 00 00 00 01 90 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 27 06 00 00 27 10 00 00 27 10 00 00 27 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 26 AC 00 00 00 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 65 13 85 03 A5 00 00 00 00 00 03 00 00 00 09 20 00 04 45");
            Console.WriteLine(vData.DateTime);

            //var stringData = Encoding.Default.GetString(obj);
            var stringData = "TRACE 2020/05/14 11:04:32<=20-05-11 11:04:26 谐波" + "\r" +
                             "80 01 14 05 0E 0C 06 37 00 00 00 00 00 00 00 00 00 03 95 94 00 00 00 00 00 00 00 00 00 00 00 72 00 00 00 01 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 01 90 00 00 00 00 00 00 00 00 00 00 01 90 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 27 06 00 00 27 10 00 00 27 10 00 00 27 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 26 AC 00 00 00 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 65 13 85 03 A5 00 00 00 00 00 03 00 00 00 09 20 00 04 45";


            var bb = stringData.Split('\r');

            vData.ParseData(bb[1]);

            IicCurrentDemandData currentDemand = new IicCurrentDemandData();
            var stringDemamd =
                " 80 20 00 00 01 5A 26 51 2B 6C 00 00 00 00 00 00 00 00 00 00 00 42 26 50 17 18 00 00 01 5A 26 51 2B 6C 00 00 00 46 26 8F F2 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 26 8F F1 94 00 00 00 00 00 00 00 00 00 00 00 01 26 8F 71 24 00 00 00 05 26 51 2B 6C 00 00 00 05 26 8F F1 94 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 CD 1F";
            currentDemand.ParseData(stringDemamd.StringToByte());
        }
    }
}