using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManagementSystem.Security;

namespace DBManagementSystem.DataHandler
{
    public static class ObjectCreator
    {
        public static void CreateTable(NewConnection connection, string tableName)
        {
            
        }

        public static void CreateColumn(NewConnection connection, string columnName)
        {
            string commandString = "ALTER TABLE " + connection.ActualTable +" ADD " + columnName + " VARCHAR(20);";
            Console.WriteLine(commandString);
            SqlCommand sqlCmd = new SqlCommand(commandString, connection.Connection);
            sqlCmd.ExecuteNonQuery();
        }

        public static void CreateDatabase(NewConnection connection, string databaseName)
        {

        }
    }
}
