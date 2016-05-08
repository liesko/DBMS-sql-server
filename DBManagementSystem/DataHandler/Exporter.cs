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
            foreach(Table tbl in db.Tables)
            {
                if (tbl.Name==connection.ActualTable)
                {
                    ScriptingOptions options = new ScriptingOptions();
                    options.ClusteredIndexes = true;
                    options.Default = true;
                    options.DriAll = true;
                    options.Indexes = true;
                    options.IncludeHeaders = true;

                    StringCollection coll = tbl.Script(options);
                    foreach (string str in coll)
                    {
                        sb.Append(str);
                        sb.Append(Environment.NewLine);
                    }
                }
            }
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
        public static void ExportGridToPDF(NewConnection connection)
        {
            // Code Here
        }
    }
}
