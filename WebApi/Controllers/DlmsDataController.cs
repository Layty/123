using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class DlmsDataController : ApiController
    {
        [HttpGet]
        public string[] GetDataFromExcelWithAppointSheetNames()
        {
            string[] result;
            using (OleDbConnection connection = new OleDbConnection(ExcelConnectionStr))
            {
                connection.Open();
                DataTable dtSheetName = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string[] strTableNames = new string[dtSheetName.Rows.Count];
                for (int i = 0; i < dtSheetName.Rows.Count; i++)
                {
                    strTableNames[i] = dtSheetName.Rows[i]["TABLE_NAME"].ToString();
                }

                result = strTableNames;
            }

            return result;
        }

        private string ExcelConnectionStr
            = "Provider\u2002=\u2002Microsoft.Jet.OLEDB.4.0;Data Source=" +
              "C:\\Users\\Administrator\\source\\repos\\123\\WebApi\\DLMS设备信息.xls" +
              ";Extended Properties=\'Excel 8.0;IMEX=1;\'";

        public DataTable GetExcelDataTable(string excelSheetName)
        {
            DataTable result;
            using (OleDbConnection connection = new OleDbConnection(ExcelConnectionStr))
            {
                connection.Open();
                string sqlCmd = "SELECT * FROM [" + excelSheetName + "$" + "]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(sqlCmd, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                DataTable table = dataSet.Tables[0];
                result = table;
            }

            return result;
        }

        // GET: api/DlmsData/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DlmsData
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/DlmsData/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/DlmsData/5
        public void Delete(int id)
        {
        }
    }
}