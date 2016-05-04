using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using DBManagementSystem.DataHandler;
using DBManagementSystem.Security;
using DBManagementSystem.View;

namespace DBManagementSystem
{
    public partial class Form1 : Form
    {
        private NewConnection _connection;

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

            comboBox1.DataSource = _connection.Databases;
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
            _connection.ActualDatabase = comboBox1.GetItemText(this.comboBox1.SelectedItem);
            GetTables(_connection.ActualDatabase);
            Console.WriteLine(_connection.ActualDatabase);
            ChangeComboBox2();
        }

        private void ChangeComboBox2()
        {
            comboBox2.DataSource = _connection.Tables;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox2.DataSource = _connection.Tables;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            _connection.CheckedColumns = new List<string>();
            foreach (object itemChecked in checkedListBox1.CheckedItems)
            {
                _connection.CheckedColumns.Add(itemChecked.ToString());
            }

            FillGrid();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            _connection.ActualTable = comboBox2.GetItemText(this.comboBox2.SelectedItem);
            GetColumns(_connection.ActualTable);
            checkedListBox1.Items.Clear();
            foreach (String tableName in _connection.Columns)
            {
                checkedListBox1.Items.Insert(0, tableName);
            }
        }

        private void FillGrid()
        {

            // vytvorenie prikazu
            string command = "select ";

            if (_connection.IsConnected)
            {
                for(int i = 0; i < _connection.CheckedColumns.Count - 1; i++)
                {
                    command += _connection.CheckedColumns[i] + ", ";
                }
                command += _connection.CheckedColumns.Last() + " from " + _connection.ActualTable;
            }
            Console.WriteLine(command);

            // pripojenie a selectovanie
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command, _connection.Connection);

            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);

            dataGridView1.ReadOnly = true;
            dataGridView1.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string objekt = comboBox1.GetItemText(this.comboBox1.SelectedItem);
            string meno = textBox1.Text;

            if (objekt == "Table")
            {
                Console.WriteLine("Vytvarame tabulu");
            } else if (objekt == "Database")
            {
                Console.WriteLine("Vytvarame db");
            }
            else if (objekt == "Column")
            {
                Console.WriteLine("Vytvarame stlpec");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new ExportDialog().ShowDialog("Export Data");
            Exporter.Export(dataGridView1);
        }
    }
}
