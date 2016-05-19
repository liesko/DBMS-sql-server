using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DBManagementSystem.Security;

namespace DBManagementSystem.DataHandler
{
    public static class Transaction
    {
        public static SqlTransaction Trans { get; private set; }
        public static void CreateTransaction(NewConnection connection)
        {
            Trans = connection.Connection.BeginTransaction("Transaction from 1");
        }

        public static void CommitTransaction()
        {
            Trans.Commit();
        }

        public static void RollbackTransaction()
        {
            Trans.Rollback();
        }
    }
}
