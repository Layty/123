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

            IicDemandData currentDemand = new IicDemandData();
            var stringDemand =
                " 80 20 00 00 01 5A 26 51 2B 6C 00 00 00 00 00 00 00 00 00 00 00 42 26 50 17 18 00 00 01 5A 26 51 2B 6C 00 00 00 46 26 8F F2 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 26 8F F1 94 00 00 00 00 00 00 00 00 00 00 00 01 26 8F 71 24 00 00 00 05 26 51 2B 6C 00 00 00 05 26 8F F1 94 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 CD 1F";
            currentDemand.ParseData(stringDemand.StringToByte());


            var stringHarmonic =
                "80 30 19 F2 00 97 10 43 00 75 12 6C 00 A4 05 15 00 BD 03 5E 00 4B 03 79 00 4A 02 E4 00 7E 01 97 00 5D 01 6E 00 52 01 E0 00 30 01 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 7D 00 0A 00 DB 00 01 01 1B 00 04 00 3A 00 00 00 47 00 03 00 3B 00 03 00 11 00 01 00 1A 00 04 00 3D 00 00 00 07 00 04 00 15 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A3 85";
            var bytes = stringHarmonic.StringToByte();
            IicHarmonicData iicHarmonicDataUa = new IicHarmonicData();
            IicHarmonicData iicHarmonicDataUb = new IicHarmonicData();
            IicHarmonicData iicHarmonicDataUc = new IicHarmonicData();
            IicHarmonicData iicHarmonicDataIa = new IicHarmonicData();
            IicHarmonicData iicHarmonicDataIb = new IicHarmonicData();
            IicHarmonicData iicHarmonicDataIc = new IicHarmonicData();
            var resultUa = iicHarmonicDataUa.ParseData(bytes.Skip(2).Take(42).ToArray());
            var resultUb = iicHarmonicDataUb.ParseData(bytes.Skip(44).Take(42).ToArray());
            var resultUc = iicHarmonicDataUc.ParseData(bytes.Skip(86).Take(42).ToArray());
            var resultIa = iicHarmonicDataIa.ParseData(bytes.Skip(128).Take(42).ToArray());
            var resultIb = iicHarmonicDataIb.ParseData(bytes.Skip(170).Take(42).ToArray());
            var resultIc = iicHarmonicDataIc.ParseData(bytes.Skip(212).Take(42).ToArray());
        }
    }
}