using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Sales : BaseModel
    {
        static string tableQuery = @"CREATE TABLE sales (
        id INT PRIMARY KEY identity,
        prescription_id INT not null,          
        sale_date DATE default getdate(),
        FOREIGN KEY (prescription_id) REFERENCES prescriptions(id));";

        static string tableValuesQuery = @"INSERT INTO sales (prescription_id, sale_date) 
        VALUES 
        (1,'2025-02-20'), 
        (2,'2025-02-21'), 
        (3,'2025-02-22');";
        public Sales(SqlConnection connection) : base("sales", tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
