using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManagementSystem.Security;
using Microsoft.SqlServer.Management.Smo;

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

        public static void restoreDb(NewConnection connection)
        {
            Server srv = new Server();
            //Database db = new Database();
            //db = srv.Databases[connection.ActualDatabase];

            Restore restoreDB = new Restore();
            restoreDB.Database = "master";
            /* Specify whether you want to restore database, files or log */
            restoreDB.Action = RestoreActionType.Database;
            restoreDB.Devices.AddDevice(@"C:\\CSV\\FullBackUp.bak", DeviceType.File);

            /* You can specify ReplaceDatabase = false (default) to not create a new
             * database, the specified database must exist on SQL Server
             * instance. If you can specify ReplaceDatabase = true to create new
             * database image regardless of the existence of specified database with
             * the same name */
            restoreDB.ReplaceDatabase = true;

            /* If you have a differential or log restore after the current restore,
             * you would need to specify NoRecovery = true, this will ensure no
             * recovery performed and subsequent restores are allowed. It means it
             * the database will be in a restoring state. */
            restoreDB.NoRecovery = true;

            /* Wiring up events for progress monitoring */
            //restoreDB.PercentComplete += CompletionStatusInPercent;
            //restoreDB.Complete += Restore_Completed;

            /* SqlRestore method starts to restore the database
             * You can also use SqlRestoreAsync method to perform restore 
             * operation asynchronously */
            restoreDB.SqlRestore(srv);
            //restoreDB.SqlRestoreAsync(srv);
        }

    }
}
