using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;

namespace Naif.TestUtilities
{
    public class DataUtil
    {
        public static void CreateDatabase(string databaseName)
        {
            if (File.Exists(databaseName))
            {
                File.Delete(databaseName);
            }

            var engine = new SqlCeEngine(GetConnectionString(databaseName));
            engine.CreateDatabase();
            engine.Dispose();
        }

        public static void DeleteDatabase(string databaseName)
        {
            if (File.Exists(databaseName))
            {
                File.Delete(databaseName);
            }
        }

        public static void ExecuteNonQuery(string databaseName, string sqlScript)
        {
            ExecuteNonQuery(databaseName, sqlScript, (cmd) => cmd.ExecuteNonQuery());
        }

        public static SqlCeDataReader ExecuteReader(string databaseName, string sqlScript)
        {
            return ExecuteQuery(databaseName, sqlScript, (cmd) => cmd.ExecuteReader());
        }

        public static int ExecuteScalar(string databaseName, string sqlScript)
        {
            return ExecuteQuery(databaseName, sqlScript, (cmd) => (int)cmd.ExecuteScalar());
        }

        public static string GetConnectionString(string databaseName)
        {
            return String.Format("Data Source = {0};", databaseName);
        }

        public static int GetLastAddedRecordID(string databaseName, string tableName, string primaryKeyField)
        {
            return ExecuteScalar(databaseName, String.Format(DataResources.GetLastAddedRecordID, tableName, primaryKeyField));
        }

        public static int GetRecordCount(string databaseName, string tableName)
        {
            return ExecuteScalar(databaseName, String.Format(DataResources.RecordCountScript, tableName));
        }

        public static DataTable GetTable(string databaseName, string tableName)
        {
            return ExecuteQuery<DataTable>(databaseName, String.Format(DataResources.GetTable, tableName),
                (cmd) =>
                {
                    var reader = cmd.ExecuteReader();
                    var table = new DataTable();
                    table.Load(reader);
                    return table;
                });
        }

        public static DataTable GetRecordsByField(string databaseName, string tableName, string fieldName, string fieldValue)
        {
            var reader = ExecuteReader(databaseName, String.Format(DataResources.GetRecordsByField, tableName, fieldName, fieldValue));
            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        private static TReturn ExecuteQuery<TReturn>(string databaseName, string sqlScript, Func<SqlCeCommand, TReturn> command)
        {
            using (SqlCeConnection connection = new SqlCeConnection(GetConnectionString(databaseName)))
            {
                connection.Open();

                using (SqlCeCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlScript;
                    return command(cmd);
                }
            }
        }

        private static void ExecuteNonQuery(string databaseName, string sqlScript, Action<SqlCeCommand> command)
        {
            using (SqlCeConnection connection = new SqlCeConnection(GetConnectionString(databaseName)))
            {
                connection.Open();

                using (SqlCeCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlScript;
                    command(cmd);
                }
            }
        }
    }
}
