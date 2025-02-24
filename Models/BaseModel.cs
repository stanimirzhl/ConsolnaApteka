using Microsoft.Data.SqlClient;

namespace Models
{
    public abstract class BaseModel
    {
        private string tableQuery;
        private string tableValuesQuery;

        public BaseModel(string tableQuery, string tableValuesQuery, SqlConnection connection)
        {
            this.tableQuery = tableQuery;
            this.tableValuesQuery = tableValuesQuery;

            this.CreateTable(connection);
            this.InsertData(connection);
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
