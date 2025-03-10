using Microsoft.Data.SqlClient;
using Models;

namespace Database
{
    public class DatabaseInvoker
    {
        public static void CreateDb(string databaseName, bool flag)
        {
            string masterConnectionString = @"Server=.\SQLEXPRESS;Database=master;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";
            //string masterConnectionString = @"Server=(localdb)\ProjectModels;Database=master;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

            using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
            {
                masterConnection.Open();

                if (DatabaseExists(masterConnection, databaseName))
                {
                    if (flag)
                    {
                        Console.WriteLine("Database already exists, use that one!" + "\n");
                        return;
                    }
                    return;
                }
                if (!flag)
                {
                    Console.WriteLine("Database didn't exist so i created it for you ;)");
                }
                string createDatabaseQuery = $"CREATE DATABASE {databaseName}";
                ExecuteQuery(masterConnection, createDatabaseQuery);
                Console.WriteLine("Database successfully created." + "\n");
            }
            masterConnectionString = masterConnectionString.Replace("master", databaseName);
            using (SqlConnection connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                var tables = new BaseModel[]
                {
                    new Categories(connection),
                    new Manufacturers(connection),
                    new Doctors(connection),
                    new Patients(connection),
                    new Prescriptions(connection),
                    new Medicines(connection),
                    new Prescription_Medicines(connection),
                    new Sales(connection),
                };
            }
        }
        static bool DatabaseExists(SqlConnection connection, string databaseName)
        {
            string query = $"SELECT database_id FROM sys.databases WHERE Name = @databaseName";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@databaseName", databaseName);
                return command.ExecuteScalar() != null;
            }
        }
        static void ExecuteQuery(SqlConnection connection, string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
