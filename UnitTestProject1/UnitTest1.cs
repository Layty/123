using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using 三相智慧能源网关调试软件.Model.IIC;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IICInstantData vData=new IICInstantData();
            vData.ParseData(" 80 01 14 05 0E 0C 06 37 00 00 00 00 00 00 00 00 00 03 95 94 00 00 00 00 00 00 00 00 00 00 00 72 00 00 00 01 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 01 90 00 00 00 00 00 00 00 00 00 00 01 90 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 27 06 00 00 27 10 00 00 27 10 00 00 27 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 26 AC 00 00 00 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 65 13 85 03 A5 00 00 00 00 00 03 00 00 00 09 20 00 04 45");
            Console.WriteLine(vData.DateTime);

            //var stringData = Encoding.Default.GetString(obj);
          var  stringData = "TRACE 2020/05/14 11:04:32<=20-05-11 11:04:26 谐波"+"\r"+ "80 01 14 05 0E 0C 06 37 00 00 00 00 00 00 00 00 00 03 95 94 00 00 00 00 00 00 00 00 00 00 00 72 00 00 00 01 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 01 90 00 00 00 00 00 00 00 00 00 00 01 90 00 00 68 B0 00 00 00 00 00 00 00 00 00 00 68 B0 00 00 27 06 00 00 27 10 00 00 27 10 00 00 27 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 26 AC 00 00 00 26 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 65 13 85 03 A5 00 00 00 00 00 03 00 00 00 09 20 00 04 45";


          var bb = stringData.Split('\r');

            vData.ParseData(bb[1]);
        }
    }
}
