using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDlmsStandard.ApplicationLay.Association;
using 三相智慧能源网关调试软件;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.Model.ENetConfig;
using 三相智慧能源网关调试软件.Model.IIC;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

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
            iicHarmonicDataUa.ParseData(bytes.Skip(2).Take(42).ToArray());
            iicHarmonicDataUb.ParseData(bytes.Skip(44).Take(42).ToArray());
            iicHarmonicDataUc.ParseData(bytes.Skip(86).Take(42).ToArray());
            iicHarmonicDataIa.ParseData(bytes.Skip(128).Take(42).ToArray());
            iicHarmonicDataIb.ParseData(bytes.Skip(170).Take(42).ToArray());
            iicHarmonicDataIc.ParseData(bytes.Skip(212).Take(42).ToArray());
        }

        [TestMethod]
        public void TestExcel()
        {
            ExcelHelper excel = new ExcelHelper("DLMS设备信息");
            excel.GetDataFromExcelWithAppointSheetNames();
            excel.GetExcelDataTable("Register");
        }


        [TestMethod]
        public void TestENetEventType()
        {
            ENetMessageBuilder messageMaker = new ENetMessageBuilder(ENetEventType.运行状态字);
            messageMaker.Timestamp = 1593316175641;
            messageMaker.GetRequest();
        }

        [TestMethod]
        public void TestAssociationResponse()
        {
            string str =
                "61 29 A1 09 06 07 60 85 74 05 08 01 01 A2 03 02 01 01 A3 05 A1 03 02 01 00 BE 10 04 0E 08 00 06 5F 1F 04 00 00 3E 9D 08 00 00 07 B1 F3 7E";
            var bytes = str.StringToByte();
            new AssociationResponse().PduStringInHexConstructor(ref str);
        }

        [TestMethod]
        public void TestLoadData()
        {
            string str =
                "A0 A0 2E00 18 02 09 2040 00 00 00 02 35 00 00 01 40 00 00 00 00 0E 00 79 0001 00 00 AA 15 00 000113 06 00 00000170 02 0201 00 00AA83 E5";

            var load = new LoadIdentificationViewModel.LoadDataParse();
            if (load.Parse(str.Replace(" ", "")))
            {
                var LoadDataFormat = load.ToString();
            }
        }
    }
}