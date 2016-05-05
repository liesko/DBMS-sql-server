using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManagementSystem.Security;

namespace DBManagementSystem.DataHandler
{
    public static class DataFilter
    {
        public static string Select(NewConnection connection, string condition)
        {
            string commandString = "SELECT ";
            foreach (var col in connection.CheckedColumns)
            {
                commandString += col;
                if (connection.CheckedColumns.Last() != col)
                {
                    commandString += ", ";
                }
            }
                
            commandString += " FROM " + connection.ActualTable + " " + condition + ";";
            Console.WriteLine(commandString);
            //SqlCommand sqlCmd = new SqlCommand(commandString, connection.Connection);
            //sqlCmd.ExecuteNonQuery();
            return commandString;
        }
    }
}
