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
            //Build the CSV file data as a Comma separated string.
            string csv = string.Empty;
            DataGridView dataGridView1 = dataGridView;

            //Add the Header row for CSV file.
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                csv += column.HeaderText + ',';
            }

            //Add new line.
            csv += "\r\n";

            //Adding the Rows
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    //Add the Data rows.
                    if (cell.Value != null)
                    {
                        csv += cell.Value.ToString().Replace(",", ";") + ',';
                    }

                }

                //Add new line.
                csv += "\r\n";
            }

            //Exporting to CSV.
            string folderPath = "C:\\CSV\\";
            File.WriteAllText(folderPath + FileName +".csv", csv);
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
