using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBManagementSystem
{
    public partial class Form1 : Form
    {
        SqlConnection connection;
        private List<string> databases;
        private List<string> tables;
        private List<string> columns;
        private string actualDatabase, actualTable;

        public Form1(SqlConnection pConnection)
        {
            this.connection = pConnection;
            InitializeComponent();
            databases = new List<string>();
            tables = new List<string>();
            columns = new List<string>();
            actualDatabase = "";
            actualTable = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // connect to a database
            if(isConnected(connection))
            {
                getDatabases();
            }
            
            //databases.Add("Prva databaza");
            //databases.Add("Druha databaza");
            comboBox1.DataSource = databases;
            
        }

        public void getDatabases()
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            SqlConnection _c = connection;

            cmd.CommandText = "SELECT name FROM master.dbo.sysdatabases";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                databases.Add(Convert.ToString(reader["name"]));
                Console.WriteLine(Convert.ToString(reader["name"]));
            }
            connection.Close();
        }

        public void getTables(String databaseName)
        {
            string connectionString = "Data Source=127.0.0.1,3306; Database=" + databaseName + "; User ID = sa; Password = oracle";
            connection = new SqlConnection(connectionString);
            //connection.ChangeDatabase(databaseName);

            tables = new List<string>();

            if (isConnected(connection))
            {
                DataTable dt = connection.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    string tablename = (string)row[2];
                    tables.Add(tablename);
                }
                connection.Close();
            }
        }

        public void getColumns(String tableName)
        {
            columns = new List<string>();

            if (isConnected(connection))
            {
                string[] restrictions = new string[4] { null, null, tableName, null };

                //connection.ChangeDatabase(actualDatabase);

                var columnList =
                    connection.GetSchema("Columns", restrictions)
                        .AsEnumerable()
                        .Select(s => s.Field<String>("Column_Name"))
                        .ToList();
                foreach (var columnName in columnList)
                {
                    columns.Add(columnName);
                    Console.WriteLine(columnName);
                }
                connection.Close();
            }
        }

        public bool isConnected(SqlConnection connectionToTest)
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
            actualDatabase = comboBox1.GetItemText(this.comboBox1.SelectedItem);
            getTables(actualDatabase);
            Console.WriteLine(actualDatabase);
            changeComboBox2();
        }

        private void changeComboBox2()
        {
            comboBox2.DataSource = tables;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox2.DataSource = tables;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            actualTable = comboBox2.GetItemText(this.comboBox2.SelectedItem);
            getColumns(actualTable);

        }
    }
}
