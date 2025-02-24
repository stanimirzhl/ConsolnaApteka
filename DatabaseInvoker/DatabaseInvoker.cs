using Microsoft.Data.SqlClient;
using Models;

namespace Database
{
    public class DatabaseInvoker
    {
        public static void CreateDb(string databaseName)
        {
            string masterConnectionString = @"Server=(localdb)\ProjectModels;Database=master;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
            {
                masterConnection.Open();

                if (DatabaseExists(masterConnection, databaseName))
                {
                    Console.WriteLine("Database already exists, use that one!" + "\n");
                    return;
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
