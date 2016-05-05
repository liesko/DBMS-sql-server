using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBManagementSystem.Security;

namespace DBManagementSystem.DataHandler
{
    public static class ObjectUpdater
    {
        public static void UpdateColumn(NewConnection connection, string condition)
        {
            string command = "UPDATE " + connection.ActualTable + condition;
            Console.WriteLine(command);
            SqlCommand sqlCmd = new SqlCommand(command, connection.Connection);
            sqlCmd.ExecuteNonQuery();
        }
    }
}
