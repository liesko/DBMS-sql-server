using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

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

            SqlConnection connection = new SqlConnection("Data Source=127.0.0.1,3306;Initial Catalog=DatabazoveJazyky;User ID=sa;Password=oracle");

            Application.Run(new Form1(connection));
        }
    }
}
