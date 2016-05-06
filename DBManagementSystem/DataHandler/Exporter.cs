using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using DBManagementSystem.Security;

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
                StreamWriter swOut = new StreamWriter("C:\\CSV\\DataGridViewExport.csv");

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
                //conn.Close();
                conn.Dispose();
                xmlFileData += ds.GetXml();
            }
            File.WriteAllText("C:/CSV/exportXML.xml", xmlFileData);
        }
    }
}
