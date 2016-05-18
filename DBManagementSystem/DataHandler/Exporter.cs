using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using DBManagementSystem.Security;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.SqlEnum;
//using Microsoft.SqlServer.Management.Smo.CoreEnum;
using System.Configuration;
using System.Collections.Specialized;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using Database = Microsoft.SqlServer.Management.Smo.Database;

namespace DBManagementSystem.DataHandler
{
    class Exporter
    {
        public static string FileName { get; set; }
        public static string Path { get; set; }

        public static void ExportCSV(DataGridView dataGridView)
        {
            //test to see if the DataGridView has any rows
            if (dataGridView.RowCount > 0)
            {
                string value = "";
                DataGridViewRow dr = new DataGridViewRow();
                // StreamWriter swOut = new StreamWriter("C:\\CSV\\DataGridViewExport.csv"); Exporter.FileName
                StreamWriter swOut = new StreamWriter("C:\\CSV\\"+Exporter.FileName); 
                //write header rows to csv
                for (int i = 0; i <= dataGridView.Columns.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        swOut.Write(",");
                    }
                    swOut.Write(dataGridView.Columns[i].HeaderText);
                }

                swOut.WriteLine();

                //write DataGridView rows to csv
                for (int j = 0; j <= dataGridView.Rows.Count - 2; j++)
                {
                    if (j > 0)
                    {
                        swOut.WriteLine();
                    }

                    dr = dataGridView.Rows[j];

                    for (int i = 0; i <= dataGridView.Columns.Count - 1; i++)
                    {
                        if (i > 0)
                        {
                            swOut.Write(",");
                        }
                        value = dr.Cells[i].Value.ToString();
                        //replace comma's with spaces
                        value = value.Replace(',', ' ');
                        //replace embedded newlines with spaces
                        value = value.Replace(Environment.NewLine, " ");

                        swOut.Write(value);

                    }
                }
                swOut.Close();
            }
        }

        public static void ExportXML(NewConnection connection)
        {
            var connStr = connection.Connection.ConnectionString;
            var xmlFileData = "";
            DataSet ds = new DataSet();
            var tables = new[] { connection.ActualTable };
            foreach (var table in tables)
            {

                var query = "SELECT * FROM " + table;
                SqlConnection conn = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                conn.Dispose();
                xmlFileData += ds.GetXml();
            }
            File.WriteAllText("C:\\CSV\\" + Exporter.FileName, xmlFileData);
        }

        public static void ExportSQL(NewConnection connection)
        {
            Server srv = new Server();
            Database db = new Database();
            db = srv.Databases[connection.ActualDatabase];

            StringBuilder sb = new StringBuilder();
            // create TABLE script - creation
            // EXPORT NA CREATE TABLE SA NEKONA!!!! ale fungovalo to...
            /*
            foreach(CrystalDecisions.CrystalReports.Engine.Table tbl in db.Tables)
            {
                if (tbl.Name==connection.ActualTable)
                {
                    ScriptingOptions options = new ScriptingOptions();
                    options.ClusteredIndexes = true;
                    options.Default = true;
                    options.DriAll = true;
                    options.Indexes = true;
                    options.IncludeHeaders = true;

                    StringCollection coll= tbl.Script(options);
                    foreach (string str in coll)
                    {
                        sb.Append(str);
                        sb.Append(Environment.NewLine);
                    }
                }
            }
            */
            // list of insert script - creation
            sb.Append(Environment.NewLine);
            var connStr = connection.Connection.ConnectionString;
            DataSet ds = new DataSet();
            var tables = new[] { connection.ActualTable };
            foreach (var table in tables)
            {
                var query = "SELECT * FROM " + table;
                SqlConnection conn = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                // --------------------------------
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    sb.AppendFormat("INSERT INTO " + connection.ActualTable + " VALUES (");
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        sb.AppendFormat("'" + row[i].ToString() + "'");
                        if (i < ds.Tables[0].Columns.Count - 1)
                        {
                            sb.AppendFormat(",");
                        }
                        else
                        {
                            sb.AppendFormat(")\n");
                        }
                    }
                    sb.Append(Environment.NewLine);
                }
                // --------------------------------
                // list of insert script - END
                conn.Dispose();
            }
            System.IO.StreamWriter fs = System.IO.File.CreateText("C:\\CSV\\" + Exporter.FileName);
            fs.Write(sb.ToString());
            fs.Close();
        }
        public static void ExportGridToPDF(NewConnection connection, CrystalReportViewer crystalReportVieverParam)
        {
            ReportDocument cryRpt = new ReportDocument();
            cryRpt.Load("C:\\Users\\A616758\\Desktop\\DBMS_SQL_SERVER\\DBMS-sql-server\\DBManagementSystem\\CrystalReport3.rpt");
            crystalReportVieverParam.ReportSource= cryRpt;
            crystalReportVieverParam.Refresh();
        }

        public static void BackUpTable(NewConnection connection)
        {
            Server srv = new Server();
            //Database db = new Database();
            //db = srv.Databases[connection.ActualDatabase];


            Backup bkpDBFull = new Backup();
            /* Specify whether you want to back up database or files or log */
            bkpDBFull.Action = BackupActionType.Database;
            /* Specify the name of the database to back up */
            //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            //Console.WriteLine(connection.Databases);
            bkpDBFull.Database = "master";
            /* You can take backup on several media type (disk or tape), here I am
             * using File type and storing backup on the file system */
            bkpDBFull.Devices.AddDevice(@"C:\\CSV\\FullBackUp.bak", DeviceType.File);
            bkpDBFull.BackupSetName = "Adventureworks database Backup";
            bkpDBFull.BackupSetDescription = "Adventureworks database - Full Backup";
            /* You can specify the expiration date for your backup data
             * after that date backup data would not be relevant */
            bkpDBFull.ExpirationDate = DateTime.Today.AddDays(10);

            /* You can specify Initialize = false (default) to create a new 
             * backup set which will be appended as last backup set on the media. You
             * can specify Initialize = true to make the backup as first set on the
             * medium and to overwrite any other existing backup sets if the all the
             * backup sets have expired and specified backup set name matches with
             * the name on the medium */
            bkpDBFull.Initialize = false;

            /* SqlBackup method starts to take back up
             * You can also use SqlBackupAsync method to perform the backup 
             * operation asynchronously */
            bkpDBFull.SqlBackup(srv);
        }
    }
}
