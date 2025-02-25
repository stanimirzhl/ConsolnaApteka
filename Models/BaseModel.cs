using Microsoft.Data.SqlClient;

namespace Models
{
    public abstract class BaseModel
    {
        private string tableQuery;
        private string tableValuesQuery;
        private string tableName;

        public BaseModel(string tableName,string tableQuery, string tableValuesQuery, SqlConnection connection)
        {
            this.tableQuery = tableQuery;
            this.tableValuesQuery = tableValuesQuery;
            this.tableName = tableName;

            this.CreateTable(connection);
            this.InsertData(connection);
            Console.WriteLine($"Table {this.tableName} has been successfully created and values has been inserted." + "\n");
        }

        private void CreateTable(SqlConnection connection)
        {
            using (SqlCommand command = new SqlCommand(tableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        private void InsertData(SqlConnection connection)
        {
            using (SqlCommand command = new SqlCommand(tableValuesQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
