using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManagementSystem.Security;

namespace DBManagementSystem.DataHandler
{
    public static class Importer
    {
        public static void ImportData(NewConnection connection, string path)
        {
            string conString = "Data Source=127.0.0.1,3306;Initial Catalog=DatabazoveJazyky;User ID=sa;Password=oracle";
            SqlConnection con = new SqlConnection(conString);
            string filepath = "C:\\CSV\\import.csv";
            StreamReader sr = new StreamReader(filepath);
            string line = sr.ReadLine();
            string[] value = line.Split(',');
            DataTable dt = new DataTable();
            DataRow row;
            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                }
            }
            SqlBulkCopy bc = new SqlBulkCopy(conString, SqlBulkCopyOptions.TableLock);
            bc.DestinationTableName = connection.ActualTable;
            bc.BatchSize = dt.Rows.Count;
            con.Open();
            bc.WriteToServer(dt);
            bc.Close();
        }
    }
}
