using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBManagementSystem
{
    public partial class Form1 : Form
    {
        SqlConnection connection;
        private List<string> databases;
        private List<string> tables;
        public Form1(SqlConnection pConnection)
        {
            this.connection = pConnection;
            InitializeComponent();
            databases = new List<string>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // connect to a database
            if(isConnected())
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

            cmd.CommandText = "SELECT name FROM master.dbo.sysdatabases";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;

            connection.Open();

            reader = cmd.ExecuteReader();
            // Data is accessible through the DataReader object here.

            while (reader.Read())
            {
                databases.Add(Convert.ToString(reader["name"]));
                Console.WriteLine(Convert.ToString(reader["name"]));
            }

            connection.Close();
            SqlCommand getDBs = new SqlCommand();

            
        }

        public bool isConnected()
        {
            try
            {
                connection.Open();
                connection.Close();
                MessageBox.Show("Connection has been established", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (SqlException exc)
            {
                MessageBox.Show(exc.StackTrace + "", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
