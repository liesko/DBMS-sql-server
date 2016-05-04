using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DBManagementSystem.Security;
using DBManagementSystem.View;

namespace DBManagementSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            string promptValue = new LoginDialog().ShowDialog("Login Form");
            //"Data Source=127.0.0.1,3306;Initial Catalog=DatabazoveJazyky;User ID=sa;Password=oracle"

            SqlConnection connection = new SqlConnection(promptValue);
            NewConnection myConnection = new NewConnection(connection);
            if (myConnection.IsConnected)
            {
                Application.Run(new Form1(myConnection));
            }

        }
    }
}
