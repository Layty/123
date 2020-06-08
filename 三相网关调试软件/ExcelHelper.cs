using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace 三相智慧能源网关调试软件
{
    public class ExcelHelper
    {
        private string ExcelConnectionStr { get; set; }
        private readonly string _fileName; //文件名
        public ExcelHelper(string excelFileName)
        {
            ExcelConnectionStr = "Provider\u2002=\u2002Microsoft.Jet.OLEDB.4.0;Data Source=" + excelFileName +
                                 ";Extended Properties=\'Excel 8.0;IMEX=1;\'";
            _fileName = excelFileName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        public DataTable GetExcelDataTable(string excelSheetName)
        {
            DataTable result;
            using (OleDbConnection connection = new OleDbConnection(ExcelConnectionStr))
            {
                connection.Open();
                string sqlCmd = "SELECT * FROM [" + excelSheetName + "]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(sqlCmd, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                DataTable table = dataSet.Tables[0];
                result = table;
            }

            return result;
        }



        public void SaveDataTableToExcelFile(string excelSheetName, DataTable dataTable)
        {
            DataTable result;
            using (OleDbConnection connection = new OleDbConnection(ExcelConnectionStr))
            {
                connection.Open();
                string strSqlcmd = "SELECT * FROM [" + excelSheetName + "]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(strSqlcmd, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                adapter.Fill(dataTable);
                DataTable table = dataSet.Tables[0];
            }
        }


        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }
   

    /// <summary>
    /// 将DataTable数据导入到excel中
    /// </summary>
    /// <param name="data">要导入的数据</param>
    /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
    /// <param name="sheetName">要导入的excel的sheet的名称</param>
    /// <returns>导入数据行数(包含列名那一行)</returns>
    public void DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            using (FileStream fs = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                IWorkbook workbook = null;
                if (_fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (_fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);
                ISheet sheet;
                if (workbook != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }
                }
                else
                {
                    return;
                }

                var count = 0;
                int j;
                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }

                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (int i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }

                    ++count;
                }

                using
                    (FileStream fs2 = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var bw = fs2.CanWrite;
                    workbook.Write(fs2); //写入到excel
                }
            }
        }


        public void Export(DataGrid dataGrid, string excelTitle)
        {
            DataTable dt = new System.Data.DataTable();
            for (int i = 0;
                i < dataGrid.Columns.Count;
                i++)
            {
                if (dataGrid.Columns[i].Visibility == Visibility.Visible) //只导出可见列  
                {
                    dt.Columns.Add(dataGrid.Columns[i].Header.ToString()); //构建表头  
                }
            }

            for (int i = 0;
                i < dataGrid.Items.Count;
                i++)
            {
                int columnsIndex = 0;
                DataRow row = dt.NewRow();
                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    if (dataGrid.Columns[j].Visibility == Visibility.Visible)
                    {
                        if (dataGrid.Items[i] != null &&
                            (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock) != null) //填充可见列数据  
                        {
                            row[columnsIndex] = (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock)
                                .Text;
                        }
                        else row[columnsIndex] = "";

                        columnsIndex++;
                    }
                }

                dt.Rows.Add(row);
            }
        }
    }
}