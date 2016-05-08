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
            StreamReader sr = new StreamReader(path);
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
            DataTableToDbTable(connection, dt);
        }
        public static void ImportXMLData(NewConnection connection, string path)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(path);
            DataTable dt = ds.Tables[0];
            DataTableToDbTable(connection, dt);
        }

        public static void ImportSQLData(NewConnection connection, string path)
        {
            StreamReader reader = new StreamReader(path);
            string line;

            while (!reader.EndOfStream)
            {
                line = reader.ReadToEnd();
                var command = connection.Connection.CreateCommand();
                command.CommandText = line;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }

        private static void DataTableToDbTable(NewConnection connection, DataTable dt)
        {
            string conString = connection.Connection.ConnectionString;
            SqlConnection con = new SqlConnection(conString);

            SqlBulkCopy bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);
            foreach (DataColumn col in dt.Columns)
            {
                bc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            }
            bc.DestinationTableName = connection.ActualTable;
            bc.WriteToServer(dt);
        }

    }
}
