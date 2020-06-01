using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectTest.HelperMethods
{
    class ExcelUtil
    {
        public static DataTable ExcelToDataTable(string filename)
        {
            FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelreader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelreader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
            excelreader.Close();
            excelreader.Dispose();
            stream.Close();
            stream.Dispose();
            return result.Tables["Sheet1"];

        }

    }
}
