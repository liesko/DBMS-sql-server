using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManagementSystem.Security
{
    public class NewConnection
    {
        private string _user;
        private string _password;
        private string _ip;
        private string _catalog;
        public bool IsConnected { get; private set; }

        public System.Data.SqlClient.SqlConnection Connection { get; set; }
        public List<string> Databases { get; set; }
        public List<string> Tables { get; set; }
        public List<string> Columns { get; set; }
        public List<string> CheckedColumns { get; set; }
        public string ActualDatabase { get; set; }
        public string ActualTable { get; set; }

    public NewConnection(string user, string password, string ip, string catalog)
        {
            this._user = user;
            this._password = password;
            this._ip = ip;
            this._catalog = catalog;
            //string connectionString = "Data Source=" + _ip + ";Initial Catalog=" + _catalog + ";User ID=" + _user + ";Password=" + _password;
            this.Connection = new SqlConnection("Data Source=127.0.0.1,3306;Initial Catalog=DatabazoveJazyky;User ID=sa;Password=oracle");
            //Console.WriteLine(connectionString);
            Connect();
        }

        public NewConnection(SqlConnection conn)
        {
            this.Connection = new SqlConnection("Data Source=127.0.0.1,3306;Initial Catalog=DatabazoveJazyky;User ID=sa;Password=oracle");
            Connect();

        }

        public void CheckConnection()
        {
            try
            {
                Connection.Open();
                IsConnected = true;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                IsConnected = false;
                Console.WriteLine("SOMETHIG REALLY WRONG HAPPENED");
            }
        }

        public void Connect()
        {
            try
            {
                Connection.Open();
                IsConnected = true;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                IsConnected = false;
                Console.WriteLine("WTFFFFFFFFFFFFFFFFFFFFFFFFFFf");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void ChangeDatabaseName(string databaseName)
        {
            //string connectionString = "Data Source=127.0.0.1,3306; Database=" + databaseName + "; User ID = sa; Password = oracle";
            //Connection = new SqlConnection(connectionString);
            //Connection.Open();
            Connection.ChangeDatabase(databaseName);
        }
    }
}
