using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DBManagementSystem.Security;

namespace DBManagementSystem.DataHandler
{
    public static class ObjectRemover
    {
        public static void DeleteColumns(NewConnection connection)
        {
            List<string> stlpce = connection.CheckedColumns;
            foreach (string stlpec in stlpce)
            {
                string commandString = "ALTER TABLE " + connection.ActualTable + " DROP COLUMN " + stlpec + ";";
                SqlCommand sqlCmd = new SqlCommand(commandString, connection.Connection);
                sqlCmd.ExecuteNonQuery();
            }
        }

        public static void DeleteTable(NewConnection connection)
        {
            string commandString = "DROP TABLE " + connection.ActualTable + ";";
            SqlCommand sqlCmd = new SqlCommand(commandString, connection.Connection);
            sqlCmd.ExecuteNonQuery();
        }

        public static void DeleteDatabase(NewConnection connection)
        {
            string commandString =  @"
                ALTER DATABASE " + connection.ActualDatabase + @" SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [" + connection.ActualDatabase + "]"; 
            SqlCommand sqlCmd = new SqlCommand(commandString, connection.Connection);
            //connection.ActualDatabase = connection.Databases.First();
            connection.ChangeDatabaseName(connection.Databases.First());
            sqlCmd.ExecuteNonQuery();
        }
    }
}
