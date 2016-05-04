using System.Windows.Forms;
using System.IO;

namespace DBManagementSystem.DataHandler
{
    class Exporter
    {
        public static string FileName { get; set; }
        public static string Path { get; set; }

        public static void Export(DataGridView dataGridView)
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
    }
}
