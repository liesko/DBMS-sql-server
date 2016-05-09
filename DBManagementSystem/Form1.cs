using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using DBManagementSystem.DataHandler;
using DBManagementSystem.Security;
using DBManagementSystem.View;
using CrystalDecisions.Windows.Forms;

namespace DBManagementSystem
{
    public partial class Form1 : Form
    {
        private NewConnection _connection;
        private SqlDataAdapter dataAdapter;
        private DataSet set;
        public Form1(NewConnection connection)
        {
            this._connection = connection;
            InitializeComponent();
            _connection.Databases = new List<string>();
            _connection.Tables = new List<string>();
            _connection.Columns = new List<string>();
            _connection.CheckedColumns = new List<string>();
            _connection.ActualDatabase = "";
            _connection.ActualTable = "";
            InitializeApplication();
        }

        private void InitializeApplication()
        {

            if (_connection.IsConnected)
            {
                GetDatabases();
            }

            databaseCombo.DataSource = _connection.Databases;
        }

        public void GetDatabases()
        {
            SqlConnection c = _connection.Connection;
            SqlCommand cmd = new SqlCommand("SELECT name FROM master.dbo.sysdatabases");
            //cmd.CommandType = CommandType.Text;
            cmd.Connection = c;

            SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    _connection.Databases.Add(Convert.ToString(reader["name"]));
                    Console.WriteLine(Convert.ToString(reader["name"]));
                }
                reader.Close();
        }

        public void GetTables(String databaseName)
        {
            _connection.ChangeDatabaseName(databaseName);

            _connection.Tables = new List<string>();

            if (_connection.IsConnected)
            {
                DataTable dt = _connection.Connection.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    string tablename = (string)row[2];
                    _connection.Tables.Add(tablename);
                }
            }
        }

        public void GetColumns(String tableName)
        {
            _connection.Columns = new List<string>();

            if (_connection.IsConnected)
            {
                string[] restrictions = new string[4] { null, null, tableName, null };

                //_connection.ChangeDatabaseName(_connection.ActualDatabase);

                var columnList =
                    _connection.Connection.GetSchema("Columns", restrictions)
                        .AsEnumerable()
                        .Select(s => s.Field<String>("Column_Name"))
                        .ToList();
                foreach (var columnName in columnList)
                {
                    _connection.Columns.Add(columnName);
                    Console.WriteLine(columnName);
                }
            }
        }

        public bool IsConnected(SqlConnection connectionToTest)
        {
            try
            {
                connectionToTest.Open();
                //connectionToTest.Close();
                //MessageBox.Show("Connection has been established", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (SqlException exc)
            {
                MessageBox.Show(exc.StackTrace + "", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _connection.ActualDatabase = databaseCombo.GetItemText(this.databaseCombo.SelectedItem);
            GetTables(_connection.ActualDatabase);
            Console.WriteLine(_connection.ActualDatabase);
            ChangeComboBox2();
        }

        private void ChangeComboBox2()
        {
            tablesCombo.DataSource = _connection.Tables;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tablesCombo.DataSource = _connection.Tables;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            _connection.CheckedColumns = new List<string>();
            foreach (object itemChecked in checkedListBox1.CheckedItems)
            {
                _connection.CheckedColumns.Add(itemChecked.ToString());
            }

            FillGrid(ActualCommand());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            _connection.ActualTable = tablesCombo.GetItemText(this.tablesCombo.SelectedItem);
            GetColumns(_connection.ActualTable);
            checkedListBox1.Items.Clear();
            foreach (String tableName in _connection.Columns)
            {
                checkedListBox1.Items.Insert(0, tableName);
            }
        }

        private void FillGrid(string command)
        {
            // pripojenie a selectovanie
            dataAdapter = new SqlDataAdapter(command, _connection.Connection);

            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

            set = new System.Data.DataSet();


            //DataTable table = new DataTable();
            //table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(set);



            dataGridView1.ReadOnly = false;
            dataGridView1.DataSource = set.Tables[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string objekt = objectTypeCombo.GetItemText(objectTypeCombo.SelectedItem);
            string meno = nameTextBox.Text;

            if (objekt == "Table")
            {
                ObjectCreator.CreateTable(_connection, meno);
            } else if (objekt == "Database")
            {
                ObjectCreator.CreateDatabase(_connection, meno);
            }
            else if (objekt == "Column")
            {
                string columnType = columnTypeCombo.GetItemText(columnTypeCombo.SelectedItem);
                bool autoIncrement = autoIncrementCheckBox.Checked;
                bool pk = columnPropsCombo.GetItemText(columnPropsCombo.SelectedItem) == "Primary Key" ? true : false;
                bool unique = columnPropsCombo.GetItemText(columnPropsCombo.SelectedItem) == "Unique" ? true : false;
                //WriteLine(meno + columnType + "|"+ autoIncrement + unique +pk);
                ObjectCreator.CreateColumn(_connection, meno, columnType, autoIncrement, unique, pk);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string type = importExportCombo.GetItemText(importExportCombo.SelectedItem);
            if (type == "CSV")
            {
                new ExportDialog().ShowDialog("Export data to CSV from DatagridView and TABLE: "+_connection.ActualTable);
                Exporter.ExportCSV(dataGridView1);
            } else if (type == "XML")
            {
                new ExportDialog().ShowDialog("ExportXML Data from actual TABLE:"+_connection.ActualTable);
                Exporter.ExportXML(_connection);
            }
            else if (type == "SQL")
            {
                new ExportDialog().ShowDialog("Export SQL (insert) from actual TABLE:" + _connection.ActualTable);
                Exporter.ExportSQL(_connection);
            }
            else if (type == "Grid2PDF")
            {
                //new ExportDialog().ShowDialog("Export DataGridView to PDF");
                Exporter.ExportGridToPDF(_connection, crystalReportViewer1);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            _connection.CheckedColumns = new List<string>();
            foreach (object itemChecked in checkedListBox1.CheckedItems)
            {
                _connection.CheckedColumns.Add(itemChecked.ToString());
            }
            ObjectRemover.DeleteColumns(_connection);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ObjectRemover.DeleteDatabase(_connection);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            ObjectRemover.DeleteTable(_connection);
            ChangeComboBox2();
            tablesCombo.Refresh();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "where [column] < [operand]";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string condition = textBox2.Text;
            string command = DataFilter.Select(_connection, condition);
            FillGrid(command);
        }

        private string ActualCommand()
        {
            // vytvorenie prikazu
            string command = "select ";

            if (_connection.IsConnected)
            {
                for (int i = 0; i < _connection.CheckedColumns.Count - 1; i++)
                {
                    command += _connection.CheckedColumns[i] + ", ";
                }
                command += _connection.CheckedColumns.Last() + " from " + _connection.ActualTable;
            }
            return command;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog(); // Show the dialog.
            string type = importExportCombo.GetItemText(importExportCombo.SelectedItem.ToString());
            if (type == "CSV")
            {
                Importer.ImportData(_connection, openFileDialog.FileName);
            }
            else if (type == "XML")
            {
                Importer.ImportXMLData(_connection, openFileDialog.FileName);
            }
            else if (type == "SQL")
            {
                Importer.ImportSQLData(_connection, openFileDialog.FileName);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string command = textBox3.Text;

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                SqlCommandBuilder bulder = new SqlCommandBuilder(dataAdapter);
                dataAdapter.Update(set);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type = importExportCombo.GetItemText(importExportCombo.SelectedItem.ToString());
            if (type == "Grid2PDF")
            {
                importBtn.Enabled = false;
            }
            else
            {
                importBtn.Enabled = true;
            }
        }

        private void openFileDialog2_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
